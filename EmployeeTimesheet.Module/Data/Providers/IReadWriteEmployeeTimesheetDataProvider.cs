namespace EmployeeTimesheet.Module.Data.Providers
{
    public interface IReadWriteEmployeeTimesheetDataProvider : IReadEmployeeTimesheetDataProvider
    {
        Task<long> CreateTimesheetAsync(Data.Models.EmployeeTimesheetDataModel timesheet);
    }
}

