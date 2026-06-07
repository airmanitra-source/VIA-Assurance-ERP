using Dapper;
using FileTable.Infrastructure.FileTableDb;
using CompanyDocuments.Module.Data.Models;
using CompanyDocuments.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanyDocumentReadOnly : ICompanyDocumentReadOnlyDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public CompanyDocumentReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CompanyDocumentDataModel>> ReadAllDocumentsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT 
                    d.stream_id as StreamId,
                    d.name as Name,
                    d.file_type as FileType,
                    d.cached_file_size as CachedFileSize,
                    d.creation_time as CreationTime,
                    d.last_write_time as LastWriteTime,
                    d.last_access_time as LastAccessTime,
                    d.is_directory as IsDirectory,
                    d.is_offline as IsOffline,
                    d.is_hidden as IsHidden,
                    d.is_readonly as IsReadonly,
                    d.is_archive as IsArchive,
                    d.is_system as IsSystem,
                    d.is_temporary as IsTemporary,
                    cd.TypeDocument, 
                    cd.IsSigned,
                    cd.SignedDate,
                    cd.EntrepriseFleetID, 
                    cd.EntrepriseWarehouseID, 
                    cd.EntrepriseMerchandiseTransportationID
                FROM [documentdb].[dbo].[Documents] d
                INNER JOIN [documentdb].[dbo].[CompanyDocuments] cd ON d.stream_id = cd.FileStreamID";
            var results = await connection.QueryAsync<CompanyDocumentDataModel>(sql);
            return results.ToList();
        }

        public async Task<CompanyDocumentDataModel?> ReadDocumentByIdAsync(Guid streamId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT 
                    d.stream_id as StreamId,
                    d.name as Name,
                    d.file_type as FileType,
                    d.cached_file_size as CachedFileSize,
                    d.creation_time as CreationTime,
                    d.last_write_time as LastWriteTime,
                    d.last_access_time as LastAccessTime,
                    d.is_directory as IsDirectory,
                    d.is_offline as IsOffline,
                    d.is_hidden as IsHidden,
                    d.is_readonly as IsReadonly,
                    d.is_archive as IsArchive,
                    d.is_system as IsSystem,
                    d.is_temporary as IsTemporary,
                    cd.TypeDocument, 
                    cd.IsSigned,
                    cd.SignedDate,
                    cd.EntrepriseFleetID, 
                    cd.EntrepriseWarehouseID, 
                    cd.EntrepriseMerchandiseTransportationID 
                FROM [documentdb].[dbo].[Documents] d
                INNER JOIN [documentdb].[dbo].[CompanyDocuments] cd ON d.stream_id = cd.FileStreamID
                WHERE d.stream_id = @StreamId";
            return await connection.QueryFirstOrDefaultAsync<CompanyDocumentDataModel>(sql, new { StreamId = streamId });
        }

        public async Task<List<CompanyDocumentDataModel>> ReadDocumentsByEntrepriseIdAsync(long entrepriseId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT 
                    d.stream_id as StreamId,
                    d.name as Name,
                    d.file_type as FileType,
                    d.cached_file_size as CachedFileSize,
                    d.creation_time as CreationTime,
                    d.last_write_time as LastWriteTime,
                    d.last_access_time as LastAccessTime,
                    d.is_directory as IsDirectory,
                    d.is_offline as IsOffline,
                    d.is_hidden as IsHidden,
                    d.is_readonly as IsReadonly,
                    d.is_archive as IsArchive,
                    d.is_system as IsSystem,
                    d.is_temporary as IsTemporary,
                    cd.TypeDocument, 
                    cd.IsSigned,
                    cd.SignedDate,
                    cd.EntrepriseFleetID, 
                    cd.EntrepriseWarehouseID, 
                    cd.EntrepriseMerchandiseTransportationID 
                FROM [documentdb].[dbo].[Documents] d
                INNER JOIN [documentdb].[dbo].[CompanyDocuments] cd ON d.stream_id = cd.FileStreamID
                WHERE cd.EntrepriseID = @EntrepriseID";
            var results = await connection.QueryAsync<CompanyDocumentDataModel>(sql, new { EntrepriseID = entrepriseId });
            return results.ToList();
        }

        public async Task<byte[]?> ReadFileContentAsync(Guid streamId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = "SELECT file_stream FROM [documentdb].[dbo].[Documents] WHERE stream_id = @StreamId";
            return await connection.QueryFirstOrDefaultAsync<byte[]>(sql, new { StreamId = streamId });
        }
    }
}

