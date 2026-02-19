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

        public async Task<long> AddEmployeeAsync(EmployeeBusinessModel employee)
        {
            var dataModel = MapToDataModel(employee);
            return await _employeeReadWrite.CreateEmployeeAsync(dataModel);
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
            var employee = await _employeeReadWrite.GetEmployeeByIdAsync(id);
            return employee != null ? MapToBusinessModel(employee) : null;
        }

        private static EmployeeDataModel MapToDataModel(EmployeeBusinessModel businessModel)
        {
            return new EmployeeDataModel
            {
                EmployeeID = businessModel.EmployeeID,
                Nom = businessModel.Nom,
                Prenom = businessModel.Prenom,
                Age = businessModel.Age,
                Sexe = businessModel.Sexe,
                NomPoste = businessModel.NomPoste,
                Fonctions = businessModel.Fonctions,
                NombreMoisPoste = businessModel.NombreMoisPoste,
                StatutEmploye = businessModel.StatutEmploye,
                EntrepriseID = businessModel.EntrepriseID,
                IsActive = businessModel.IsActive,
                NumeroMatricule = businessModel.NumeroMatricule,
                DateFinContrat = businessModel.DateFinContrat,
                VouloirSouscrire = businessModel.VouloirSouscrire
            };
        }

        private static EmployeeBusinessModel MapToBusinessModel(EmployeeDataModel dataModel)
        {
            return new EmployeeBusinessModel
            {
                EmployeeID = dataModel.EmployeeID,
                Nom = dataModel.Nom,
                Prenom = dataModel.Prenom,
                Age = dataModel.Age,
                Sexe = dataModel.Sexe,
                NomPoste = dataModel.NomPoste,
                Fonctions = dataModel.Fonctions,
                NombreMoisPoste = dataModel.NombreMoisPoste,
                StatutEmploye = dataModel.StatutEmploye,
                EntrepriseID = dataModel.EntrepriseID,
                IsActive = dataModel.IsActive,
                NumeroMatricule = dataModel.NumeroMatricule,
                DateFinContrat = dataModel.DateFinContrat,
                VouloirSouscrire = dataModel.VouloirSouscrire
            };
        }
    }
}
