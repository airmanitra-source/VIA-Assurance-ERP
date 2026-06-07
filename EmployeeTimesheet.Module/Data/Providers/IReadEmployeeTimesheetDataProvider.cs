using EmployeeTimesheet.Module.Data.Models;

namespace EmployeeTimesheet.Module.Data.Providers
{
    public interface IReadEmployeeTimesheetDataProvider
    {
        Task<List<EmployeeTimesheetDataModel>> ReadTimesheetsByEmployeeIdAsync(long employeeId);
    }
}

