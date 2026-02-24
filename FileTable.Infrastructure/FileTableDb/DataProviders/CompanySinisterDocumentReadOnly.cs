using Dapper;
using CompanySinisterDocument.Module.Data.Models;
using CompanySinisterDocument.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class CompanySinisterDocumentReadOnly : ICompanySinisterDocumentReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public CompanySinisterDocumentReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<CompanySinisterDocumentDataModel>> ReadAllDocumentsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT 
                    d.[stream_id] AS StreamId,
                    d.[name] AS Name,
                    d.[path_locator].ToString() AS PathLocator,
                    d.[parent_path_locator].ToString() AS ParentPathLocator,
                    d.[file_type] AS FileType,
                    d.[cached_file_size] AS CachedFileSize,
                    d.[creation_time] AS CreationTime,
                    d.[last_write_time] AS LastWriteTime,
                    d.[last_access_time] AS LastAccessTime,
                    d.[is_directory] AS IsDirectory,
                    d.[is_offline] AS IsOffline,
                    d.[is_hidden] AS IsHidden,
                    d.[is_readonly] AS IsReadonly,
                    d.[is_archive] AS IsArchive,
                    d.[is_system] AS IsSystem,
                    d.[is_temporary] AS IsTemporary
                FROM [documentdb].[dbo].[Documents] d
                ORDER BY d.[is_directory] DESC, d.[name]";

            var documents = await connection.QueryAsync<CompanySinisterDocumentDataModel>(sql);
            return documents.ToList();
        }

        public async Task<CompanySinisterDocumentDataModel?> ReadDocumentByIdAsync(Guid streamId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT 
                    d.[stream_id] AS StreamId,
                    d.[name] AS Name,
                    d.[path_locator].ToString() AS PathLocator,
                    d.[parent_path_locator].ToString() AS ParentPathLocator,
                    d.[file_type] AS FileType,
                    d.[cached_file_size] AS CachedFileSize,
                    d.[creation_time] AS CreationTime,
                    d.[last_write_time] AS LastWriteTime,
                    d.[last_access_time] AS LastAccessTime,
                    d.[is_directory] AS IsDirectory,
                    d.[is_offline] AS IsOffline,
                    d.[is_hidden] AS IsHidden,
                    d.[is_readonly] AS IsReadonly,
                    d.[is_archive] AS IsArchive,
                    d.[is_system] AS IsSystem,
                    d.[is_temporary] AS IsTemporary
                FROM [documentdb].[dbo].[Documents] d
                WHERE d.[stream_id] = @StreamId";

            return await connection.QueryFirstOrDefaultAsync<CompanySinisterDocumentDataModel>(sql, new { StreamId = streamId });
        }

        public async Task<List<CompanySinisterDocumentDataModel>> ReadDocumentsBySinisterIdAsync(long sinisterId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT 
                    d.[stream_id] AS StreamId,
                    d.[name] AS Name,
                    d.[path_locator].ToString() AS PathLocator,
                    d.[parent_path_locator].ToString() AS ParentPathLocator,
                    d.[file_type] AS FileType,
                    d.[cached_file_size] AS CachedFileSize,
                    d.[creation_time] AS CreationTime,
                    d.[last_write_time] AS LastWriteTime,
                    d.[last_access_time] AS LastAccessTime,
                    d.[is_directory] AS IsDirectory,
                    d.[is_offline] AS IsOffline,
                    d.[is_hidden] AS IsHidden,
                    d.[is_readonly] AS IsReadonly,
                    d.[is_archive] AS IsArchive,
                    d.[is_system] AS IsSystem,
                    d.[is_temporary] AS IsTemporary,
                    csd.TypeDocument
                FROM [documentdb].[dbo].[Documents] d
                INNER JOIN [documentdb].[dbo].[CompanySinisterDocuments] csd ON d.[stream_id] = csd.FileStreamID
                WHERE csd.CompanySinisterID = @CompanySinisterID
                ORDER BY d.[name]";

            var documents = await connection.QueryAsync<CompanySinisterDocumentDataModel>(sql, new { CompanySinisterID = sinisterId });
            return documents.ToList();
        }

        public async Task<byte[]?> ReadFileContentAsync(Guid streamId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT [file_stream]
                FROM [documentdb].[dbo].[Documents]
                WHERE [stream_id] = @StreamId AND [is_directory] = 0";

            return await connection.QueryFirstOrDefaultAsync<byte[]>(sql, new { StreamId = streamId });
        }
    }
}
