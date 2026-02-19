using Company.Module.Business;

namespace Company.Module
{
    public interface ICompanyModule
    {
        Task<EntrepriseBusinessModel?> GetCompanyByEmailAsync(string email);
        Task<EntrepriseBusinessModel?> GetCompanyByIdAsync(long id);
    }
}
