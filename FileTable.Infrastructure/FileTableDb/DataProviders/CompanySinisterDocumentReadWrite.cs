using Dapper;
using CompanySinisterDocument.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanySinisterDocumentReadWrite : CompanySinisterDocumentReadOnly, ICompanySinisterDocumentReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public CompanySinisterDocumentReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateCompanySinisterDocumentAsync(long entrepriseId, long sinisterId, Guid fileStreamId, string? typeDocument)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[CompanySinisterDocuments] (CompanySinisterID, EntrepriseID, FileStreamID, TypeDocument)
                VALUES (@CompanySinisterID, @EntrepriseID, @FileStreamID, @TypeDocument)";

            await connection.ExecuteAsync(sql, new
            {
                CompanySinisterID = sinisterId,
                EntrepriseID = entrepriseId,
                FileStreamID = fileStreamId,
                TypeDocument = typeDocument
            });
        }

        public async Task<Guid> CreateCompanySinisterFileAsync(string fileName, byte[] fileContent)
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

        public async Task DeleteCompanySinisterDocumentAsync(long entrepriseId, long sinisterId, Guid streamId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var deleteLinkSql = @"
                    DELETE FROM [documentdb].[dbo].[CompanySinisterDocuments]
                    WHERE EntrepriseID = @EntrepriseID AND CompanySinisterID = @CompanySinisterID AND FileStreamID = @FileStreamID";

                await connection.ExecuteAsync(deleteLinkSql, new
                {
                    EntrepriseID = entrepriseId,
                    CompanySinisterID = sinisterId,
                    FileStreamID = streamId
                }, transaction);

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
