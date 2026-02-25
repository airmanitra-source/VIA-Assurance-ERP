using Employee.Module.Business;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;

namespace Employee.Module
{
    public class EmployeeModule : IEmployeeModule
    {
        private readonly IEmployeeReadWrite _employeeReadWrite;

        public EmployeeModule(IEmployeeReadWrite employeeReadWrite)
        {
            _employeeReadWrite = employeeReadWrite;
        }

        public async Task<long> AddEmployeeAsync(EmployeeBusinessModel employee, int? projectId = null)
        {
            var dataModel = MapToDataModel(employee);
            return await _employeeReadWrite.CreateEmployeeAsync(dataModel, projectId);
        }

        public async Task<IEnumerable<EmployeeBusinessModel>> GetEmployeesByEnterpriseIdAsync(long enterpriseId)
        {
            var employees = await _employeeReadWrite.ReadEmployeesByEnterpriseAsync(enterpriseId);
            return employees.Select(MapToBusinessModel);
        }

        public async Task SetEmployeeAsync(EmployeeBusinessModel employee)
        {
            var dataModel = MapToDataModel(employee);
            await _employeeReadWrite.UpdateEmployeeAsync(dataModel);
        }

        public async Task SetEmployeeActiveStatusAsync(long employeeId, bool isActive, DateTime? dateFinContrat = null)
        {
            await _employeeReadWrite.UpdateEmployeeActiveStatusAsync(employeeId, isActive, dateFinContrat);
        }

        public async Task<IEnumerable<EmployeeBusinessModel>> GetEmployeesByUserIdAsync(string userId)
        {
            if (long.TryParse(userId, out var enterpriseId))
            {
                var employees = await _employeeReadWrite.ReadEmployeesByEnterpriseAsync(enterpriseId);
                return employees.Select(MapToBusinessModel);
            }
            return Enumerable.Empty<EmployeeBusinessModel>();
        }

        public async Task<EmployeeBusinessModel?> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeReadWrite.ReadEmployeeByIdAsync(id);
            return employee != null ? MapToBusinessModel(employee) : null;
        }

        public async Task<EmployeeBusinessModel?> GetEmployeeByEmailAsync(string email)
        {
            var employee = await _employeeReadWrite.ReadEmployeeByEmailAsync(email);
            return employee != null ? MapToBusinessModel(employee) : null;
        }

        private static EmployeeDataModel MapToDataModel(EmployeeBusinessModel businessModel)
        {
            return new EmployeeDataModel
            {
                Age = businessModel.Age,
                DateEmbauche = businessModel.DateEmbauche,
                DateFinContrat = businessModel.DateFinContrat,
                EmployeeID = businessModel.EmployeeID,
                Email = businessModel.Email,
                EntrepriseID = businessModel.EntrepriseID,
                Fonctions = businessModel.Fonctions,
                IsActive = businessModel.IsActive,
                Nom = businessModel.Nom,
                NombreMoisPoste = businessModel.NombreMoisPoste,
                NomPoste = businessModel.NomPoste,
                NumeroMatricule = businessModel.NumeroMatricule,
                Prenom = businessModel.Prenom,
                Sexe = businessModel.Sexe,
                StatutEmploye = businessModel.StatutEmploye,
                VouloirSouscrire = businessModel.VouloirSouscrire
            };
        }

        private static EmployeeBusinessModel MapToBusinessModel(EmployeeDataModel dataModel)
        {
            return new EmployeeBusinessModel
            {
                Age = dataModel.Age,
                DateEmbauche = dataModel.DateEmbauche,
                DateFinContrat = dataModel.DateFinContrat,
                EmployeeID = dataModel.EmployeeID,
                Email = dataModel.Email,
                EntrepriseID = dataModel.EntrepriseID,
                Fonctions = dataModel.Fonctions,
                IsActive = dataModel.IsActive,
                Nom = dataModel.Nom,
                NombreMoisPoste = dataModel.NombreMoisPoste,
                NomPoste = dataModel.NomPoste,
                NumeroMatricule = dataModel.NumeroMatricule,
                Prenom = dataModel.Prenom,
                Sexe = dataModel.Sexe,
                StatutEmploye = dataModel.StatutEmploye,
                VouloirSouscrire = dataModel.VouloirSouscrire
            };
        }
    }
}
