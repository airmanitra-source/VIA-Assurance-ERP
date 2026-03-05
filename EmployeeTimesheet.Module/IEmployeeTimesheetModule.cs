using EmployeeTimesheet.Module.Business;

namespace EmployeeTimesheet.Module
{
    public interface IEmployeeTimesheetModule
    {
        Task<long> AddTimesheetAsync(EmployeeTimesheetBusinessModel timesheet);
        Task<List<EmployeeTimesheetBusinessModel>> GetTimesheetsByEmployeeIdAsync(long employeeId);
    }
}
