using NUnit.Framework;

namespace ClientApp.E2ETests;

/// <summary>
/// Centralise l'accès aux paramètres du test (URL, identifiants, chaîne de connexion...).
/// Chaque valeur est lue d'abord depuis une variable d'environnement "E2E_{Name}",
/// puis depuis les TestRunParameters du fichier .runsettings, sinon une valeur par défaut.
/// </summary>
public static class TestConfig
{
    public static string BaseUrl => Get("BaseUrl", "https://localhost:7246").TrimEnd('/');

    public static string Username => Get("Username", string.Empty);

    public static string Password => Get("Password", string.Empty);

    public static long EmployeeId => long.Parse(Get("EmployeeId", "1002"));

    public static string DocumentType => Get("DocumentType", "CV");

    public static string ConnectionString => Get(
        "ConnectionString",
        "Server=localhost\\MSSQLSERVER01;Integrated Security=True;Encrypt=Optional;Database=master;Trusted_Connection=True;TrustServerCertificate=True;");

    /// <summary>
    /// Lit un paramètre : priorité à la variable d'environnement E2E_{name}, sinon TestRunParameters, sinon défaut.
    /// </summary>
    private static string Get(string name, string defaultValue)
    {
        var fromEnv = Environment.GetEnvironmentVariable($"E2E_{name}");
        if (!string.IsNullOrWhiteSpace(fromEnv))
        {
            return fromEnv;
        }

        var fromRunSettings = TestContext.Parameters.Get(name);
        if (!string.IsNullOrWhiteSpace(fromRunSettings))
        {
            return fromRunSettings;
        }

        return defaultValue;
    }
}
