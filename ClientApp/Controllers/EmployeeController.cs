using ClientApp.Models;
using Employee.Module;
using Employee.Module.Business;
using EmployeeDocuments.Module;
using EmployeeDocuments.Module.Business;
using Project.Module;
using System.Linq;

namespace ClientApp.Controllers
{
    public class EmployeeController
    {
        private readonly IEmployeeDocumentModule _employeeDocumentModule;
        private readonly IEmployeeModule _employeeModule;
        private readonly IPayrollModule _payrollModule;
        private readonly IProjectModule _projectModule;

        public EmployeeController(
            IEmployeeDocumentModule employeeDocumentModule,
            IEmployeeModule employeeModule,
            IPayrollModule payrollModule,
            IProjectModule projectModule)
        {
            _employeeDocumentModule = employeeDocumentModule;
            _employeeModule = employeeModule;
            _payrollModule = payrollModule;
            _projectModule = projectModule;
        }

        /// <summary>
        /// REST: Index - Get all employees for an enterprise
        /// </summary>
        public async Task<List<EmployeeViewModel>> Index(long enterpriseId)
        {
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);
            return employees
                .Select(MapBusinessModelToViewModel)
                .OrderBy(e => e.Nom)
                .ThenBy(e => e.Prenom)
                .ToList();
        }

        /// <summary>
        /// REST: Index - Get all active projects for the project selector
        /// </summary>
        public async Task<List<ProjectViewModel>> IndexProjectsAsync()
        {
            var projects = await _projectModule.GetActiveProjectsAsync();
            return projects
                .Select(p => new ProjectViewModel { ProjectID = p.ProjectID, ProjectName = p.ProjectName })
                .ToList();
        }

        /// <summary>
        /// REST: Show - Get a single employee with documents and project
        /// </summary>
        public async Task<EmployeeDetailViewModel?> Show(long employeeId, long enterpriseId)
        {
            // All business logic delegated to module
            var employeeDetail = await _employeeModule.GetEmployeeByIdAndEnterpriseAsync(employeeId, enterpriseId);

            if (employeeDetail == null)
                return null;

            var employeeViewModel = MapBusinessModelToViewModel(employeeDetail.Employee);

            if (employeeDetail.Project != null)
            {
                employeeViewModel.ProjectID = employeeDetail.Project.ProjectID;
                employeeViewModel.SelectedProjectId = employeeDetail.Project.ProjectID;
            }

            return new EmployeeDetailViewModel
            {
                Employee = employeeViewModel,
                Documents = employeeDetail.Documents
                    .Select(d => new AttachedDocumentViewModel
                    {
                        ExistingDocument = d,
                        TypeDocument = d.TypeDocument ?? "Autre"
                    })
                    .ToList()
            };
        }

        /// <summary>
        /// REST: Store - Create or Update employee with documents
        /// </summary>
        public async Task<StoreResult> Store(
            EmployeeViewModel viewModel,
            long? employeeId,
            long enterpriseId,
            List<AttachedDocumentViewModel> documents,
            List<Guid> documentsToDelete)
        {
            var result = new StoreResult();

            try
            {
                // Capture old salary if updating
                decimal? oldSalary = null;
                if (employeeId.HasValue)
                {
                    var existingDetail = await _employeeModule.GetEmployeeByIdAndEnterpriseAsync(employeeId.Value, enterpriseId);
                    oldSalary = existingDetail?.Employee.Salaire;
                }

                // Map ViewModel to Business Model
                var businessModel = MapViewModelToBusinessModel(viewModel, employeeId, enterpriseId);

                long savedEmployeeId;
                if (employeeId.HasValue)
                {
                    await _employeeModule.SetEmployeeAsync(businessModel);
                    savedEmployeeId = employeeId.Value;
                    result.Message = $"Employee {viewModel.Prenom} {viewModel.Nom} updated successfully!";

                    // PaySlip Invalidation Check: If salary changed, check for draft payslips
                    if (oldSalary.HasValue && viewModel.Salaire != oldSalary.Value)
                    {
                        var periods = await _payrollModule.GetPeriodsByEnterpriseAsync(enterpriseId);
                        foreach (var period in periods.Where(p => p.Status == "Draft" || p.Status == "Open"))
                        {
                            var savedPaySlip = await _payrollModule.GetSavedPaySlipAsync(employeeId.Value, period.PeriodID);
                            if (savedPaySlip != null)
                            {
                                result.ShowPayrollWarning = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    savedEmployeeId = await _employeeModule.AddEmployeeAsync(businessModel, viewModel.SelectedProjectId);
                    result.Message = $"Employee {viewModel.Prenom} {viewModel.Nom} added successfully!";
                }

                result.EmployeeId = savedEmployeeId;

                // Process deletions
                foreach (var streamId in documentsToDelete)
                {
                    try
                    {
                        await _employeeDocumentModule.RemoveDocumentAsync(savedEmployeeId, streamId);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Failed to delete document: {ex.Message}");
                    }
                }

                // Upload new documents
                foreach (var doc in documents.Where(d => d.File != null))
                {
                    try
                    {
                        using var stream = doc.File!.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                        using var ms = new MemoryStream();
                        await stream.CopyToAsync(ms);
                        var fileContent = ms.ToArray();
                        await _employeeDocumentModule.UploadAndLinkDocumentAsync(
                            savedEmployeeId,
                            doc.File.Name,
                            fileContent,
                            doc.TypeDocument);
                    }
                    catch (Exception ex)
                    {
                        result.Errors.Add($"Failed to upload {doc.File?.Name}: {ex.Message}");
                    }
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Failed to save employee: {ex.Message}");
                result.Success = false;
            }

            return result;
        }

        /// <summary>
        /// REST: Destroy - Delete an employee
        /// </summary>
        public async Task<bool> Destroy(long employeeId)
        {
            // Implementation depends on your business rules
            // You might need to add a RemoveEmployeeAsync method to IEmployeeModule
            throw new NotImplementedException("Delete employee not yet implemented");
        }

        public async Task<bool> SetActiveStatus(long employeeId, bool isActive, DateTime? deactivationDate)
        {
            try
            {
                await _employeeModule.SetEmployeeActiveStatusAsync(employeeId, isActive, deactivationDate);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Mapping
        private EmployeeViewModel MapBusinessModelToViewModel(EmployeeBusinessModel business)
        {
            return new EmployeeViewModel
            {
                EmployeeID = business.EmployeeID,
                Age = business.Age,
                DateEmbauche = business.DateEmbauche,
                DateFinContrat = business.DateFinContrat,
                Email = business.Email,
                EntrepriseId = business.EntrepriseID,
                Fonctions = business.Fonctions ?? string.Empty,
                IsActive = business.IsActive,
                Nom = business.Nom,
                NombreMoisPoste = business.NombreMoisPoste,
                NomPoste = business.NomPoste ?? string.Empty,
                NumeroMatricule = business.NumeroMatricule ?? string.Empty,
                Prenom = business.Prenom,
                Salaire = business.Salaire,
                Sexe = business.Sexe,
                StatutEmploye = business.StatutEmploye,
                VouloirSouscrire = business.VouloirSouscrire
            };
        }

        private EmployeeBusinessModel MapViewModelToBusinessModel(
            EmployeeViewModel viewModel,
            long? employeeId,
            long enterpriseId)
        {
            return new EmployeeBusinessModel
            {
                Age = viewModel.Age,
                DateEmbauche = viewModel.DateEmbauche,
                DateFinContrat = null, // Ensure nulled if saved as active
                Email = viewModel.Email,
                EmployeeID = employeeId ?? 0,
                EntrepriseID = enterpriseId,
                Fonctions = viewModel.Fonctions,
                IsActive = true,
                Nom = viewModel.Nom,
                NombreMoisPoste = viewModel.NombreMoisPoste,
                NomPoste = viewModel.NomPoste,
                NumeroMatricule = viewModel.NumeroMatricule,
                Prenom = viewModel.Prenom,
                Salaire = viewModel.Salaire,
                Sexe = viewModel.Sexe,
                StatutEmploye = viewModel.StatutEmploye,
                VouloirSouscrire = viewModel.VouloirSouscrire
            };
        }
        #endregion
    }
}