using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IEmployeeReadWrite : IEmployeeReadOnly
    {
        Task<long> CreateEmployeeAsync(EmployeeDataModel employee);
        Task<IEnumerable<EmployeeDataModel>> ReadEmployeesByEnterpriseAsync(long enterpriseId);
        Task UpdateEmployeeAsync(EmployeeDataModel employee);
        Task UpdateEmployeeActiveStatusAsync(long employeeId, bool isActive, DateTime? dateFinContrat = null);
    }
}
