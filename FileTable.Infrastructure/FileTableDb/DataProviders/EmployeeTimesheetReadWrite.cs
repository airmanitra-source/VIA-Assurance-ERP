using Dapper;
using EmployeeTimesheet.Module.Data.Models;
using EmployeeTimesheet.Module.Data.Providers;
using FileTable.Infrastructure.FileTableDb.Entities;

namespace FileTable.Infrastructure.FileTableDb.DataProviders
{
    public class EmployeeTimesheetReadWrite : IReadWriteEmployeeTimesheet
    {
        private readonly FileTableDbContext _dbContext;

        public EmployeeTimesheetReadWrite(FileTableDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<long> CreateTimesheetAsync(EmployeeTimesheetDataModel timesheet)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                INSERT INTO [documentdb].[dbo].[EmployeeTimesheet]
                    (EmployeeID, ProjectID, WorkDate, HoursWorked, OvertimeHours, TaskDescription, Status, CreatedDate)
                VALUES
                    (@EmployeeID, @ProjectID, @WorkDate, @HoursWorked, @OvertimeHours, @TaskDescription, @Status, GETDATE());
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            return await connection.ExecuteScalarAsync<long>(sql, timesheet);
        }

        public async Task<List<EmployeeTimesheetDataModel>> ReadTimesheetsByEmployeeIdAsync(long employeeId)
        {
            using var connection = _dbContext.CreateConnection();
            var sql = @"
                SELECT * FROM [documentdb].[dbo].[EmployeeTimesheet]
                WHERE EmployeeID = @employeeId
                ORDER BY WorkDate DESC";

            var entities = await connection.QueryAsync<EmployeeTimesheetEntity>(sql, new { employeeId });
            return entities.Select(MapToModel).ToList();
        }

        private static EmployeeTimesheetDataModel MapToModel(EmployeeTimesheetEntity e)
        {
            return new EmployeeTimesheetDataModel
            {
                Comments = e.Comments,
                CreatedDate = e.CreatedDate,
                EmployeeID = e.EmployeeID,
                HoursWorked = e.HoursWorked,
                ModifiedDate = e.ModifiedDate,
                OvertimeHours = e.OvertimeHours,
                ProjectID = e.ProjectID,
                Status = e.Status,
                TaskDescription = e.TaskDescription,
                TimesheetID = e.TimesheetID,
                WorkDate = e.WorkDate
            };
        }
    }
}
