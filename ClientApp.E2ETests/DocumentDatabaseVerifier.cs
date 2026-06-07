using Dapper;
using Microsoft.Data.SqlClient;

namespace ClientApp.E2ETests;

/// <summary>
/// Vérifie physiquement, via une requête SQL directe, qu'un document uploadé a bien été
/// persisté dans la base. Le flux applicatif insère :
///   1. une ligne dans [documentdb].[dbo].[Documents]        (name, file_stream -> stream_id)
///   2. une ligne dans [documentdb].[dbo].[EmployeeDocuments] (EmployeeID, FileStreamID, TypeDocument)
/// On joint donc ces deux tables sur stream_id = FileStreamID.
/// </summary>
public sealed class DocumentDatabaseVerifier
{
    private readonly string _connectionString;

    public DocumentDatabaseVerifier(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Représente la trace physique d'un document persisté.
    /// </summary>
    public sealed record PersistedDocument(
        Guid StreamId,
        string Name,
        string? TypeDocument,
        long ContentLength);

    /// <summary>
    /// Recherche un document persisté pour un employé donné, par nom de fichier.
    /// Renvoie null si aucune ligne n'est trouvée.
    /// </summary>
    public async Task<PersistedDocument?> FindPersistedDocumentAsync(
        long employeeId,
        string fileName)
    {
        const string sql = @"
            SELECT TOP 1
                d.stream_id              AS StreamId,
                d.name                   AS Name,
                ed.TypeDocument          AS TypeDocument,
                DATALENGTH(d.file_stream) AS ContentLength
            FROM [documentdb].[dbo].[EmployeeDocuments] ed
            INNER JOIN [documentdb].[dbo].[Documents] d
                ON d.stream_id = ed.FileStreamID
            WHERE ed.EmployeeID = @EmployeeId
              AND d.name = @FileName
            ORDER BY ed.Id DESC;";

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        return await connection.QuerySingleOrDefaultAsync<PersistedDocument>(
            sql,
            new { EmployeeId = employeeId, FileName = fileName });
    }

    /// <summary>
    /// Supprime la trace d'un document (lien + fichier) après le test, pour ne pas polluer la base.
    /// Best-effort : les exceptions sont ignorées.
    /// </summary>
    public async Task CleanupDocumentAsync(long employeeId, Guid streamId)
    {
        const string sql = @"
            DELETE FROM [documentdb].[dbo].[EmployeeDocuments]
            WHERE EmployeeID = @EmployeeId AND FileStreamID = @StreamId;

            DELETE FROM [documentdb].[dbo].[Documents]
            WHERE stream_id = @StreamId;";

        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, new { EmployeeId = employeeId, StreamId = streamId });
        }
        catch
        {
            // Nettoyage best-effort : on n'échoue pas le test si la suppression échoue.
        }
    }
}
