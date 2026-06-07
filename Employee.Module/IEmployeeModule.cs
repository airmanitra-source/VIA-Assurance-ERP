using Employee.Module.Business.Models;

namespace Employee.Module
{
    public interface IEmployeeModule
    {
        Task<long> AddEmployeeAsync(EmployeeBusinessModel employee, int? projectId = null);
        Task<EmployeeBusinessModel?> GetEmployeeByEmailAsync(string email);
        Task<EmployeeBusinessModel?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDetailBusinessModel?> GetEmployeeByIdAndEnterpriseAsync(long employeeId, long enterpriseId);
        Task<IEnumerable<EmployeeBusinessModel>> GetEmployeesByEnterpriseIdAsync(long enterpriseId);
        Task<IEnumerable<EmployeeBusinessModel>> GetEmployeesByUserIdAsync(string userId);
        Task SetEmployeeActiveStatusAsync(long employeeId, bool isActive, DateTime? dateFinContrat = null);
        Task SetEmployeeAsync(EmployeeBusinessModel employee);
        Task<List<EmployeeBusinessModel>> GetEmployeesWithoutPaySlipForPeriodAsync(long enterpriseId, int periodId);
    }
}
