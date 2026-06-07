using Dapper;
using Project.Module.Data.Models;
using Project.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class ProjectReadOnly : IProjectReadOnlyDataProvider
    {
        private readonly FileTableDbContext _dbContext;

        public ProjectReadOnly(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProjectDataModel>> ReadActiveProjectsAsync()
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT ProjectID, ProjectName, Description, StartDate, EndDate, Status, CreatedDate
                FROM [documentdb].[dbo].[Project]
                WHERE Status = 'Active'
                ORDER BY ProjectName ASC";

            var entities = await connection.QueryAsync<ProjectEntity>(sql);
            return entities.Select(MapToModel).ToList();
        }

        public async Task<ProjectDataModel?> ReadProjectByEmployeeIdAsync(long employeeId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT TOP 1 p.ProjectID, p.ProjectName, p.Description, p.StartDate, p.EndDate, p.Status, p.CreatedDate
                FROM [documentdb].[dbo].[Project] p
                INNER JOIN [documentdb].[dbo].[EmployeeProject] ep ON p.ProjectID = ep.ProjectID
                WHERE ep.EmployeeID = @EmployeeID
                AND ep.IsActive = 1
                ORDER BY ep.AssignedDate DESC";

            var entity = await connection.QueryFirstOrDefaultAsync<ProjectEntity>(sql, new { EmployeeID = employeeId });
            return entity != null ? MapToModel(entity) : null;
        }

        private static ProjectDataModel MapToModel(ProjectEntity e)
        {
            return new ProjectDataModel
            {
                CreatedDate = e.CreatedDate,
                Description = e.Description,
                EndDate = e.EndDate,
                ProjectID = e.ProjectID,
                ProjectName = e.ProjectName,
                StartDate = e.StartDate,
                Status = e.Status
            };
        }
    }
}

