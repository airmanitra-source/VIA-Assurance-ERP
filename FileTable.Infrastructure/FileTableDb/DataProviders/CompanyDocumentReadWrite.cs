using Dapper;
using CompanyDocuments.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanyDocumentReadWrite : CompanyDocumentReadOnly, ICompanyDocumentReadWrite
    {
        private readonly FileTableDbContext _dbContext;

        public CompanyDocumentReadWrite(FileTableDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCompanyDocumentAsync(long entrepriseId, Guid fileStreamId, string? typeDocument, long? fleetId = null, long? warehouseId = null, long? transportationId = null)
        {
            using var connection = _dbContext.CreateConnection();
            
            // Check if document is already referenced for this enterprise
            var checkSql = "SELECT COUNT(1) FROM [documentdb].[dbo].[CompanyDocuments] WHERE EntrepriseID = @EntrepriseID AND FileStreamID = @FileStreamID";
            int exists = await connection.ExecuteScalarAsync<int>(checkSql, new { EntrepriseID = entrepriseId, FileStreamID = fileStreamId });
            
            if (exists > 0)
            {
                // Update existing link
                var updateSql = @"
                    UPDATE [documentdb].[dbo].[CompanyDocuments]
                    SET TypeDocument = @TypeDocument,
                        EntrepriseFleetID = @FleetID,
                        EntrepriseWarehouseID = @WarehouseID,
                        EntrepriseMerchandiseTransportationID = @TransportationID
                    WHERE EntrepriseID = @EntrepriseID AND FileStreamID = @FileStreamID";

                await connection.ExecuteAsync(updateSql, new
                {
                    EntrepriseID = entrepriseId,
                    FileStreamID = fileStreamId,
                    TypeDocument = typeDocument,
                    FleetID = fleetId,
                    WarehouseID = warehouseId,
                    TransportationID = transportationId
                });
            }
            else
            {
                // Insert new link
                var insertSql = @"
                    INSERT INTO [documentdb].[dbo].[CompanyDocuments] 
                    (EntrepriseID, FileStreamID, TypeDocument, EntrepriseFleetID, EntrepriseWarehouseID, EntrepriseMerchandiseTransportationID)
                    VALUES (@EntrepriseID, @FileStreamID, @TypeDocument, @FleetID, @WarehouseID, @TransportationID)";

                await connection.ExecuteAsync(insertSql, new
                {
                    EntrepriseID = entrepriseId,
                    FileStreamID = fileStreamId,
                    TypeDocument = typeDocument,
                    FleetID = fleetId,
                    WarehouseID = warehouseId,
                    TransportationID = transportationId
                });
            }
        }

        public async Task<Guid> AddCompanyFileIntoDocumentsAsync(string fileName, byte[] fileContent)
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

        public async Task UpdateDocumentSignatureAsync(long entrepriseId, Guid streamId, bool isSigned, DateTime? signedDate)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[CompanyDocuments]
                SET IsSigned = @IsSigned,
                    SignedDate = @SignedDate
                WHERE EntrepriseID = @EntrepriseID AND FileStreamID = @FileStreamID";

            await connection.ExecuteAsync(sql, new
            {
                IsSigned = isSigned,
                SignedDate = signedDate,
                EntrepriseID = entrepriseId,
                FileStreamID = streamId
            });
        }

        public async Task UpdateDocumentContentAsync(Guid streamId, byte[] fileContent)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                UPDATE [documentdb].[dbo].[Documents]
                SET file_stream = @FileStream
                WHERE stream_id = @StreamId";

            await connection.ExecuteAsync(sql, new
            {
                FileStream = fileContent,
                StreamId = streamId
            });
        }

        public async Task DeleteCompanyDocumentAsync(long entrepriseId, Guid streamId)
        {
            using var connection = _dbContext.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                var deleteLinkSql = @"
                    DELETE FROM [documentdb].[dbo].[CompanyDocuments]
                    WHERE EntrepriseID = @EntrepriseID AND FileStreamID = @FileStreamID";

                await connection.ExecuteAsync(deleteLinkSql, new { EntrepriseID = entrepriseId, FileStreamID = streamId }, transaction);

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
