using Employee.Module.Business;
using Employee.Module.Data.Models;
using Employee.Module.Data.Providers;
using EmployeeDocuments.Module;
using Project.Module;

namespace Employee.Module
{
    public class EmployeeModule : IEmployeeModule
    {
        private readonly IEmployeeDocumentModule _employeeDocumentModule;
        private readonly IEmployeeReadWrite _employeeReadWrite;
        private readonly IProjectModule _projectModule;

        public EmployeeModule(
            IEmployeeDocumentModule employeeDocumentModule,
            IEmployeeReadWrite employeeReadWrite,
            IProjectModule projectModule)
        {
            _employeeDocumentModule = employeeDocumentModule;
            _employeeReadWrite = employeeReadWrite;
            _projectModule = projectModule;
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

        public async Task<EmployeeDetailBusinessModel?> GetEmployeeByIdAndEnterpriseAsync(long employeeId, long enterpriseId)
        {
            // Get employee from enterprise
            var employees = await _employeeReadWrite.ReadEmployeesByEnterpriseAsync(enterpriseId);
            var emp = employees.FirstOrDefault(e => e.EmployeeID == employeeId);

            if (emp == null)
                return null;

            // Get documents and project in parallel
            var docsTask = _employeeDocumentModule.GetDocumentsByEmployeeIdAsync(employeeId);
            var projectTask = _projectModule.GetProjectByEmployeeIdAsync(employeeId);

            await Task.WhenAll(docsTask, projectTask);

            return new EmployeeDetailBusinessModel
            {
                Documents = docsTask.Result.ToList(),
                Employee = MapToBusinessModel(emp),
                Project = projectTask.Result
            };
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
                BankAccountNumber = businessModel.BankAccountNumber,
                Classification = businessModel.Classification,
                DateEmbauche = businessModel.DateEmbauche,
                DateFinContrat = businessModel.DateFinContrat,
                Dependents = businessModel.Dependents,
                EmployeeID = businessModel.EmployeeID,
                Email = businessModel.Email,
                EntrepriseID = businessModel.EntrepriseID,
                Fonctions = businessModel.Fonctions,
                IsActive = businessModel.IsActive,
                Nom = businessModel.Nom,
                NombreMoisPoste = businessModel.NombreMoisPoste,
                NomPoste = businessModel.NomPoste,
                NumeroCnaps = businessModel.NumeroCnaps,
                NumeroMatricule = businessModel.NumeroMatricule,
                Prenom = businessModel.Prenom,
                Salaire = businessModel.Salaire,
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
                BankAccountNumber = dataModel.BankAccountNumber,
                Classification = dataModel.Classification,
                DateEmbauche = dataModel.DateEmbauche,
                DateFinContrat = dataModel.DateFinContrat,
                Dependents = dataModel.Dependents,
                EmployeeID = dataModel.EmployeeID,
                Email = dataModel.Email,
                EntrepriseID = dataModel.EntrepriseID,
                Fonctions = dataModel.Fonctions,
                IsActive = dataModel.IsActive,
                Nom = dataModel.Nom,
                NombreMoisPoste = dataModel.NombreMoisPoste,
                NomPoste = dataModel.NomPoste,
                NumeroCnaps = dataModel.NumeroCnaps,
                NumeroMatricule = dataModel.NumeroMatricule,
                Prenom = dataModel.Prenom,
                Salaire = dataModel.Salaire,
                Sexe = dataModel.Sexe,
                StatutEmploye = dataModel.StatutEmploye,
                VouloirSouscrire = dataModel.VouloirSouscrire
            };
        }
    }
}
