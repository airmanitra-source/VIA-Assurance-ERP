using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IEmployeeReadOnly
    {
        Task<EmployeeDataModel?> GetEmployeeByIdAsync(int id);
        Task<List<EmployeeDataModel>> GetEmployeesByEntrepriseIdAsync(long entrepriseId);
    }
}
