using Employee.Module.Business;

namespace Employee.Module
{
    public interface IEmployeeModule
    {
        Task<long> AddEmployeeAsync(EmployeeBusinessModel employee);
        Task<IEnumerable<EmployeeBusinessModel>> GetEmployeesByEnterpriseIdAsync(long enterpriseId);
        Task SetEmployeeAsync(EmployeeBusinessModel employee);
        Task SetEmployeeActiveStatusAsync(long employeeId, bool isActive, DateTime? dateFinContrat = null);
        Task<IEnumerable<EmployeeBusinessModel>> GetEmployeesByUserIdAsync(string userId); // UserId = Company owner User Id
        Task<EmployeeBusinessModel?> GetEmployeeByIdAsync(int id);
    }
}
