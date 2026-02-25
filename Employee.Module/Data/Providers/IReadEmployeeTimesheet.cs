using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IReadEmployeeTimesheet
    {
        Task<List<EmployeeTimesheetDataModel>> ReadTimesheetsByEmployeeIdAsync(long employeeId);
    }
}
