# ClientApp.E2ETests — Tests End-to-End Playwright

Tests d'interface (E2E) basés sur **Playwright pour .NET** (NUnit), ciblant l'application Blazor **ERP ASSUR**.

## Scénario couvert

**`EmployeeDocumentUploadTests.Upload_Image_Document_Should_Persist_In_Database`**

1. Génère une image PNG unique (en mémoire, via ImageSharp).
2. Se connecte via `/login`.
3. Ouvre la page `/add-employee?id={EmployeeId}`.
4. Sélectionne un type de document, puis **upload l'image** via l'`<InputFile>` Blazor.
5. Vérifie que le document apparaît dans la liste « Documents associés » (UI).
6. Soumet le formulaire et attend le message de succès.
7. **Vérifie physiquement en base de données** que le fichier est persisté :
   - jointure `[dbo].[EmployeeDocuments]` ↔ `[dbo].[Documents]` sur `FileStreamID = stream_id`,
   - contrôle que `DATALENGTH(file_stream) > 0` (le binaire est bien stocké),
   - contrôle du nom et du type de document.
8. Nettoie la base (suppression du document inséré) et le fichier temporaire.

## Pré-requis

1. **Application démarrée** sur `https://localhost:7246` (instance SQL Server FileTable accessible).
   ```powershell
   cd ..\ClientApp
   dotnet run
   ```
2. **Navigateurs Playwright installés** (une seule fois) :
   ```powershell
   pwsh bin/Debug/net10.0/playwright.ps1 install
   ```
3. **Identifiants** renseignés dans `.runsettings` (ou variables d'environnement) :
   - `Username`, `Password`

## Configuration (`.runsettings`)

| Paramètre | Description | Défaut |
|-----------|-------------|--------|
| `BaseUrl` | URL de l'application | `https://localhost:7246` |
| `Username` / `Password` | Identifiants de connexion | *(vide — à renseigner)* |
| `EmployeeId` | Employé ciblé | `1002` |
| `DocumentType` | Type de document | `CV` |
| `ConnectionString` | Connexion SQL pour la vérification BD | instance locale |

> Chaque paramètre peut être surchargé par une variable d'environnement préfixée `E2E_`
> (ex. `E2E_Username`, `E2E_Password`) sans modifier le fichier.

## Exécution

```powershell
# Depuis le dossier du projet de test
dotnet test --settings .runsettings

# Ou en passant les identifiants par variables d'environnement
$env:E2E_Username = "mon.user"
$env:E2E_Password = "mon.motdepasse"
dotnet test --settings .runsettings
```

Pour **voir** le navigateur exécuter le test (débogage local), passez `<Headless>false</Headless>`
dans `.runsettings`. Par défaut, le test s'exécute en **mode headless** (sans interface), adapté à la CI.
