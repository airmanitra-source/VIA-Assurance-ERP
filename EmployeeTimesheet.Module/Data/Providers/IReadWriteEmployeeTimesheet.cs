namespace EmployeeTimesheet.Module.Data.Providers
{
    public interface IReadWriteEmployeeTimesheet : IReadEmployeeTimesheet
    {
        Task<long> CreateTimesheetAsync(Data.Models.EmployeeTimesheetDataModel timesheet);
    }
}
