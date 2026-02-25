using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IReadWriteEmployeeTimesheet : IReadEmployeeTimesheet
    {
        Task<long> CreateTimesheetAsync(EmployeeTimesheetDataModel timesheet);
    }
}
