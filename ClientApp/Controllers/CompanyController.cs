using ClientApp.Models;
using Company.Module;
using Company.Module.Business;

namespace ClientApp.Controllers
{
    /// <summary>
    /// Controller for managing company/enterprise operations.
    /// Maps business models to view models and handles non-business logic.
    /// </summary>
    public class CompanyController
    {
        private readonly ICompanyModule _companyModule;

        public CompanyController(ICompanyModule companyModule)
        {
            _companyModule = companyModule;
        }

        /// <summary>
        /// Retrieves a company by its ID and maps it to a business model.
        /// </summary>
        public async Task<EntrepriseBusinessModel?> Show(long id)
        {
            return await _companyModule.GetCompanyByIdAsync(id);
        }
    }
}
