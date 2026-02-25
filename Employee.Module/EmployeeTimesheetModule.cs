using Employee.Module.Business;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;

namespace Employee.Module
{
    public class EmployeeTimesheetModule : IEmployeeTimesheetModule
    {
        private readonly IReadWriteEmployeeTimesheet _timesheetReadWrite;

        public EmployeeTimesheetModule(IReadWriteEmployeeTimesheet timesheetReadWrite)
        {
            _timesheetReadWrite = timesheetReadWrite;
        }

        public async Task<long> AddTimesheetAsync(EmployeeTimesheetBusinessModel timesheet)
        {
            var dataModel = MapToDataModel(timesheet);
            return await _timesheetReadWrite.CreateTimesheetAsync(dataModel);
        }

        public async Task<List<EmployeeTimesheetBusinessModel>> GetTimesheetsByEmployeeIdAsync(long employeeId)
        {
            var items = await _timesheetReadWrite.ReadTimesheetsByEmployeeIdAsync(employeeId);
            return items.Select(EmployeeTimesheetBusinessModel.FromDataModel).ToList();
        }

        private static EmployeeTimesheetDataModel MapToDataModel(EmployeeTimesheetBusinessModel b)
        {
            return new EmployeeTimesheetDataModel
            {
                Comments = b.Comments,
                CreatedDate = b.CreatedDate,
                EmployeeID = b.EmployeeID,
                HoursWorked = b.HoursWorked,
                ModifiedDate = b.ModifiedDate,
                OvertimeHours = b.OvertimeHours,
                ProjectID = b.ProjectID,
                Status = b.Status,
                TaskDescription = b.TaskDescription,
                TimesheetID = b.TimesheetID,
                WorkDate = b.WorkDate
            };
        }
    }
}
