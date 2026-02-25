using Employee.Module.Data.Models;

namespace Employee.Module.Data.Providers
{
    public interface IEmployeeReadOnly
    {
        Task<EmployeeDataModel?> ReadEmployeeByEmailAsync(string email);
        Task<EmployeeDataModel?> ReadEmployeeByIdAsync(int id);
        Task<List<EmployeeDataModel>> ReadEmployeesByEntrepriseIdAsync(long entrepriseId);
    }
}
