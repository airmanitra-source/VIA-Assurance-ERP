using Dapper;
using EmployeeDocuments.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EmployeeDocumentReadWrite : EmployeeDocumentReadOnly, IEmployeeDocumentReadWriteDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public EmployeeDocumentReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddEmployeeDocumentAsync(long employeeId, Guid fileStreamId, string? typeDocument)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[EmployeeDocuments] (EmployeeID, FileStreamID, TypeDocument)
                VALUES (@EmployeeID, @FileStreamID, @TypeDocument)";

            await connection.ExecuteAsync(sql, new
            {
                EmployeeID = employeeId,
                FileStreamID = fileStreamId,
                TypeDocument = typeDocument
            });
        }

        public async Task<Guid> AddEmployeeFileIntoDocumentsAsync(string fileName, byte[] fileContent)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[Documents] ([name], [file_stream])
                OUTPUT INSERTED.[stream_id]
                VALUES (@Name, @FileStream)";

            return await connection.ExecuteScalarAsync<Guid>(sql, new
            {
                Name = fileName,
                FileStream = fileContent
            });
        }

        public async Task DeleteEmployeeDocumentAsync(long employeeId, Guid streamId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 1. Delete link from EmployeeDocuments
                var deleteLinkSql = @"
                DELETE FROM [documentdb].[dbo].[EmployeeDocuments]
                WHERE EmployeeID = @EmployeeID AND FileStreamID = @FileStreamID";

                await connection.ExecuteAsync(deleteLinkSql, new { EmployeeID = employeeId, FileStreamID = streamId }, transaction);

                // 2. Delete file from Documents table
                var deleteFileSql = @"
                DELETE FROM [documentdb].[dbo].[Documents]
                WHERE stream_id = @StreamId";

                await connection.ExecuteAsync(deleteFileSql, new { StreamId = streamId }, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}

