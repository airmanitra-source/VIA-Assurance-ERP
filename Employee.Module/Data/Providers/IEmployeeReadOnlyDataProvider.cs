using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IEmployeeReadOnlyDataProvider
    {
        Task<List<EmployeeDataModel>> ReadActiveByEnterpriseIdAsync(long enterpriseId);
        Task<List<EmployeeDataModel>> ReadByEnterpriseIdAsync(long enterpriseId);
        Task<EmployeeDataModel?> ReadEmployeeByEmailAsync(string email);
        Task<EmployeeDataModel?> ReadEmployeeByIdAsync(int id);
        Task<List<EmployeeDataModel>> ReadEmployeesWithoutPaySlipForPeriodAsync(long enterpriseId, int periodId);
    }
}

