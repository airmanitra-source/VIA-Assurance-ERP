using Company.Module.Data.Providers;
using Employee.Module;
using Employee.Module.Business;

namespace ClientApp.Services
{
    public class DataSeeder
    {
        private readonly IEmployeeModule _employeeModule;
        private readonly IEntrepriseReadOnly _entrepriseReadOnly;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(IEmployeeModule employeeModule, IEntrepriseReadOnly entrepriseReadOnly, ILogger<DataSeeder> logger)
        {
            _employeeModule = employeeModule;
            _entrepriseReadOnly = entrepriseReadOnly;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            var entreprises = await _entrepriseReadOnly.ReadAllEntreprisesAsync();
            if (entreprises.Count == 0)
                return;

            var entrepriseId = entreprises[0].Id;

            var existingEmployees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(entrepriseId);
            if (existingEmployees.Any(e => e.NumeroMatricule == "MCP-01"))
                return;

            var employee = new EmployeeBusinessModel
            {
                Age = 35,
                EntrepriseID = entrepriseId,
                IsActive = true,
                Nom = "Hendersen",
                NomPoste = "Product Owner",
                NumeroMatricule = "MCP-01",
                Prenom = "Peter",
                Sexe = "M",
                StatutEmploye = "CDI"
            };

            var employeeId = await _employeeModule.AddEmployeeAsync(employee);
            _logger.LogInformation("Seeded employee Peter Hendersen with ID {EmployeeId}.", employeeId);
        }
    }
}
