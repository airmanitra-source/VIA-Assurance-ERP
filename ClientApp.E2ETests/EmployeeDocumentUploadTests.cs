using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;

namespace ClientApp.E2ETests;

/// <summary>
/// Test E2E : upload d'un document (image PNG) sur la page /add-employee,
/// avec vérification dans l'UI ET vérification physique en base de données.
///
/// Pré-requis :
///   - L'application Blazor doit être démarrée (https://localhost:7246 par défaut).
///   - Les identifiants (Username/Password) doivent être renseignés dans .runsettings
///     ou via les variables d'environnement E2E_Username / E2E_Password.
///   - SQL Server (instance FileTable) accessible via la chaîne de connexion configurée.
/// </summary>
[TestFixture]
public class EmployeeDocumentUploadTests : PageTest
{
    private DocumentDatabaseVerifier _dbVerifier = null!;
    private string? _uploadedFilePath;

    // Ignore le certificat HTTPS de développement auto-signé.
    public override BrowserNewContextOptions ContextOptions() => new()
    {
        IgnoreHTTPSErrors = true
    };

    [SetUp]
    public void SetUp()
    {
        _dbVerifier = new DocumentDatabaseVerifier(TestConfig.ConnectionString);

        // Garde-fou : un message clair si les identifiants n'ont pas été fournis.
        if (string.IsNullOrWhiteSpace(TestConfig.Username) || string.IsNullOrWhiteSpace(TestConfig.Password))
        {
            Assert.Ignore(
                "Identifiants manquants. Renseignez Username/Password dans .runsettings " +
                "ou via les variables d'environnement E2E_Username / E2E_Password.");
        }
    }

    [Test]
    public async Task Upload_Image_Document_Should_Persist_In_Database()
    {
        // --- Arrange : génère une image PNG unique ---
        var uniqueFileName = $"e2e-upload-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():N}.png";
        _uploadedFilePath = TestImageGenerator.CreatePngFile(uniqueFileName);

        PersistedDocumentForCleanup = null;

        // --- Step 1 : Authentification ---
        await LoginAsync();

        // --- Step 2 : Ouvre la page d'édition de l'employé ciblé ---
        await Page.GotoAsync($"{TestConfig.BaseUrl}/add-employee?id={TestConfig.EmployeeId}");

        // Attend que le formulaire (section documents) soit interactif.
        var fileInput = Page.Locator("input[type='file']");
        await Expect(fileInput).ToBeVisibleAsync(new() { Timeout = 15000 });

        // --- Step 3 : Sélectionne le type de document ---
        var docTypeSelect = Page.Locator("#docType");
        await docTypeSelect.SelectOptionAsync(TestConfig.DocumentType);

        // --- Step 4 : Upload du fichier image via l'InputFile Blazor ---
        await fileInput.SetInputFilesAsync(_uploadedFilePath);

        // Le document apparaît dans la liste "Documents associés" (état en mémoire, avant submit).
        var documentItem = Page.Locator(".document-item", new() { HasTextString = uniqueFileName });
        await Expect(documentItem).ToBeVisibleAsync(new() { Timeout = 10000 });

        // --- Step 5 : Soumet le formulaire pour persister en base ---
        var submitButton = Page.Locator("button[type='submit']");
        await submitButton.ClickAsync();

        // --- Step 6 : Vérifie le message de succès dans l'UI ---
        var successMessage = Page.Locator(".success-message");
        await Expect(successMessage).ToBeVisibleAsync(new() { Timeout = 20000 });

        // --- Step 7 : Vérification PHYSIQUE en base de données ---
        var persisted = await WaitForPersistedDocumentAsync(uniqueFileName);

        Assert.That(persisted, Is.Not.Null,
            $"Le document '{uniqueFileName}' n'a pas été trouvé en base pour l'employé {TestConfig.EmployeeId}.");

        PersistedDocumentForCleanup = persisted;

        Assert.Multiple(() =>
        {
            Assert.That(persisted!.Name, Is.EqualTo(uniqueFileName),
                "Le nom du fichier persisté ne correspond pas.");
            Assert.That(persisted.ContentLength, Is.GreaterThan(0),
                "Le contenu binaire du fichier (file_stream) est vide en base.");
            Assert.That(persisted.TypeDocument, Is.EqualTo(TestConfig.DocumentType),
                "Le type de document persisté ne correspond pas à celui sélectionné.");
            Assert.That(persisted.StreamId, Is.Not.EqualTo(Guid.Empty),
                "Le stream_id généré par la FileTable est vide.");
        });

        TestContext.Out.WriteLine(
            $"✅ Document persisté : StreamId={persisted!.StreamId}, " +
            $"Name={persisted.Name}, Type={persisted.TypeDocument}, " +
            $"Taille={persisted.ContentLength} octets.");
    }

    /// <summary>
    /// Référence du document persisté, utilisée pour le nettoyage en TearDown.
    /// </summary>
    private DocumentDatabaseVerifier.PersistedDocument? PersistedDocumentForCleanup { get; set; }

    /// <summary>
    /// Effectue la connexion via la page /login (formulaire SSR statique de l'application).
    /// En cas d'échec, capture le message affiché par l'application et une capture d'écran
    /// pour transformer un échec opaque en diagnostic clair.
    /// </summary>
    private async Task LoginAsync()
    {
        await Page.GotoAsync($"{TestConfig.BaseUrl}/login", new()
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        await Page.Locator("#username").FillAsync(TestConfig.Username);
        await Page.Locator("#password").FillAsync(TestConfig.Password);
        await Page.Locator("button[type='submit']").ClickAsync();

        // Laisse le POST du formulaire se traiter (SSR statique + enhanced navigation Blazor).
        try
        {
            await Page.WaitForURLAsync(
                new Regex("^(?!.*/login).*$"),
                new() { Timeout = 15000, WaitUntil = WaitUntilState.NetworkIdle });
        }
        catch (TimeoutException)
        {
            // Toujours sur /login après 15 s → on remonte la raison réelle.
            var reason = "(aucun message d'erreur affiché sur la page)";
            var errorLocator = Page.Locator(".error-message");
            if (await errorLocator.CountAsync() > 0)
            {
                var text = (await errorLocator.First.InnerTextAsync()).Trim();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    reason = text;
                }
            }

            var shot = await CaptureScreenshotAsync("login-failure");

            Assert.Fail(
                $"La connexion a échoué : la page est restée sur /login (URL actuelle : {Page.Url}). " +
                $"Message applicatif : « {reason} ». " +
                $"Vérifiez les identifiants dans .runsettings. Capture d'écran : {shot}");
        }
    }

    /// <summary>
    /// Prend une capture d'écran horodatée et renvoie son chemin.
    /// </summary>
    private async Task<string> CaptureScreenshotAsync(string label)
    {
        var directory = Path.Combine(Path.GetTempPath(), "erp-assur-e2e", "screenshots");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, $"{label}-{DateTime.UtcNow:yyyyMMdd-HHmmss-fff}.png");

        try
        {
            await Page.ScreenshotAsync(new() { Path = path, FullPage = true });
        }
        catch
        {
            // best-effort : ne pas masquer l'erreur d'origine si la capture échoue.
        }

        return path;
    }

    /// <summary>
    /// Interroge la base avec quelques tentatives, le temps que la persistance asynchrone se termine.
    /// </summary>
    private async Task<DocumentDatabaseVerifier.PersistedDocument?> WaitForPersistedDocumentAsync(string fileName)
    {
        const int maxAttempts = 10;
        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            var found = await _dbVerifier.FindPersistedDocumentAsync(TestConfig.EmployeeId, fileName);
            if (found is not null)
            {
                return found;
            }

            await Task.Delay(500);
        }

        return null;
    }

    [TearDown]
    public async Task TearDown()
    {
        // 1. Nettoyage de la base : supprime le document inséré par le test.
        if (PersistedDocumentForCleanup is not null)
        {
            await _dbVerifier.CleanupDocumentAsync(TestConfig.EmployeeId, PersistedDocumentForCleanup.StreamId);
        }

        // 2. Supprime le fichier image temporaire.
        if (_uploadedFilePath is not null && File.Exists(_uploadedFilePath))
        {
            try
            {
                File.Delete(_uploadedFilePath);
            }
            catch
            {
                // best-effort
            }
        }
    }
}
