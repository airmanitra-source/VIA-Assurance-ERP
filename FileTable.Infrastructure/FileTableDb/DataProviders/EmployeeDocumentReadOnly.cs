using Dapper;
using FileTable.Infrastructure.FileTableDb.Entities;
using EmployeeDocuments.Module.Data.Models;
using EmployeeDocuments.Module.Data.Providers;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EmployeeDocumentReadOnly : IEmployeeDocumentReadOnly
    {
        private readonly FileTableDbContext _dbContext;

        public EmployeeDocumentReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<EmployeeDocumentDataModel>> ReadAllDocumentsAsync()
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

            var entities = await connection.QueryAsync<FileTableEntity>(sql);
            return entities.Select(ConvertToDataModel).ToList();
        }

        public async Task<List<EmployeeDocumentDataModel>> ReadDocumentsByEmployeeIdAsync(long employeeId)
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
                ed.TypeDocument
            FROM [documentdb].[dbo].[Documents] d
            INNER JOIN [documentdb].[dbo].[EmployeeDocuments] ed ON d.stream_id = ed.FileStreamID
            WHERE ed.EmployeeID = @EmployeeID
            ORDER BY d.[name]";

            var documents = await connection.QueryAsync<dynamic>(sql, new { EmployeeID = employeeId });

            return documents.Select(d => new EmployeeDocumentDataModel
            {
                StreamId = d.StreamId,
                Name = d.Name,
                PathLocator = d.PathLocator,
                ParentPathLocator = d.ParentPathLocator,
                FileType = d.FileType,
                CachedFileSize = d.CachedFileSize,
                CreationTime = d.CreationTime,
                LastWriteTime = d.LastWriteTime,
                LastAccessTime = d.LastAccessTime,
                IsDirectory = d.IsDirectory,
                IsOffline = d.IsOffline,
                IsHidden = d.IsHidden,
                IsReadonly = d.IsReadonly,
                IsArchive = d.IsArchive,
                IsSystem = d.IsSystem,
                IsTemporary = d.IsTemporary,
                TypeDocument = d.TypeDocument
            }).ToList();
        }

        public async Task<EmployeeDocumentDataModel?> ReadDocumentByIdAsync(Guid streamId)
        {
            using var connection = _dbContext.CreateConnection();

            var sql = @"
            SELECT 
                [stream_id] AS StreamId,
                [name] AS Name,
                [path_locator].ToString() AS PathLocator,
                [parent_path_locator].ToString() AS ParentPathLocator,
                [file_type] AS FileType,
                [cached_file_size] AS CachedFileSize,
                [creation_time] AS CreationTime,
                [last_write_time] AS LastWriteTime,
                [last_access_time] AS LastAccessTime,
                [is_directory] AS IsDirectory,
                [is_offline] AS IsOffline,
                [is_hidden] AS IsHidden,
                [is_readonly] AS IsReadonly,
                [is_archive] AS IsArchive,
                [is_system] AS IsSystem,
                [is_temporary] AS IsTemporary
            FROM [documentdb].[dbo].[Documents]
            WHERE [stream_id] = @StreamId";

            var entity = await connection.QueryFirstOrDefaultAsync<FileTableEntity>(sql, new { StreamId = streamId });
            return entity != null ? ConvertToDataModel(entity) : null;
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

        private static EmployeeDocumentDataModel ConvertToDataModel(FileTableEntity entity)
        {
            return new EmployeeDocumentDataModel
            {
                StreamId = entity.StreamId,
                Name = entity.Name,
                PathLocator = entity.PathLocator,
                ParentPathLocator = entity.ParentPathLocator,
                FileType = entity.FileType,
                CachedFileSize = entity.CachedFileSize,
                CreationTime = entity.CreationTime,
                LastWriteTime = entity.LastWriteTime,
                LastAccessTime = entity.LastAccessTime,
                IsDirectory = entity.IsDirectory,
                IsOffline = entity.IsOffline,
                IsHidden = entity.IsHidden,
                IsReadonly = entity.IsReadonly,
                IsArchive = entity.IsArchive,
                IsSystem = entity.IsSystem,
                IsTemporary = entity.IsTemporary
            };
        }
    }
}
