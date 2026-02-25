using Employee.Module.Business;

namespace Employee.Module
{
    public interface IEmployeeTimesheetModule
    {
        Task<long> AddTimesheetAsync(EmployeeTimesheetBusinessModel timesheet);
        Task<List<EmployeeTimesheetBusinessModel>> GetTimesheetsByEmployeeIdAsync(long employeeId);
    }
}
