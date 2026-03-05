using EmployeeTimesheet.Module.Data.Models;

namespace EmployeeTimesheet.Module.Data.Providers
{
    public interface IReadEmployeeTimesheet
    {
        Task<List<EmployeeTimesheetDataModel>> ReadTimesheetsByEmployeeIdAsync(long employeeId);
    }
}
