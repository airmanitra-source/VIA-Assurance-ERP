using ClientApp.Models;
using Employee.Module;
using Employee.Module.Business;
using EmployeeDocuments.Module;
using EmployeeDocuments.Module.Business;

namespace ClientApp.Controllers
{
    public class EmployeeController
    {
        private readonly IEmployeeModule _employeeModule;
        private readonly IEmployeeDocumentModule _employeeDocumentModule;

        public EmployeeController(
            IEmployeeModule employeeModule,
            IEmployeeDocumentModule employeeDocumentModule)
        {
            _employeeModule = employeeModule;
            _employeeDocumentModule = employeeDocumentModule;
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
        /// REST: Show - Get a single employee with documents
        /// </summary>
        public async Task<EmployeeDetailViewModel?> Show(long employeeId, long enterpriseId)
        {
            var employees = await _employeeModule.GetEmployeesByEnterpriseIdAsync(enterpriseId);
            var emp = employees.FirstOrDefault(e => e.EmployeeID == employeeId);
            
            if (emp == null)
                return null;

            var docs = await _employeeDocumentModule.GetDocumentsByEmployeeIdAsync(employeeId);

            return new EmployeeDetailViewModel
            {
                Employee = MapBusinessModelToViewModel(emp),
                Documents = docs.Select(d => new AttachedDocumentViewModel
                {
                    ExistingDocument = d,
                    TypeDocument = d.TypeDocument ?? "Autre"
                }).ToList()
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
                // Map ViewModel to Business Model
                var businessModel = MapViewModelToBusinessModel(viewModel, employeeId, enterpriseId);

                long savedEmployeeId;
                if (employeeId.HasValue)
                {
                    await _employeeModule.SetEmployeeAsync(businessModel);
                    savedEmployeeId = employeeId.Value;
                    result.Message = $"Employee {viewModel.Prenom} {viewModel.Nom} updated successfully!";
                }
                else
                {
                    savedEmployeeId = await _employeeModule.AddEmployeeAsync(businessModel);
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
                DateFinContrat = business.DateFinContrat,
                EntrepriseId = business.EntrepriseID,
                Fonctions = business.Fonctions ?? string.Empty,
                IsActive = business.IsActive,
                Nom = business.Nom,
                NombreMoisPoste = business.NombreMoisPoste,
                NomPoste = business.NomPoste ?? string.Empty,
                NumeroMatricule = business.NumeroMatricule ?? string.Empty,
                Prenom = business.Prenom,
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
                DateFinContrat = null, // Ensure nulled if saved as active
                EmployeeID = employeeId ?? 0,
                EntrepriseID = enterpriseId,
                Fonctions = viewModel.Fonctions,
                IsActive = true,
                Nom = viewModel.Nom,
                NombreMoisPoste = viewModel.NombreMoisPoste,
                NomPoste = viewModel.NomPoste,
                NumeroMatricule = viewModel.NumeroMatricule,
                Prenom = viewModel.Prenom,
                Sexe = viewModel.Sexe,
                StatutEmploye = viewModel.StatutEmploye,
                VouloirSouscrire = viewModel.VouloirSouscrire
            };
        }
        #endregion
    }
}