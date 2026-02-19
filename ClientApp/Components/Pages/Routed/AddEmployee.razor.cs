using ClientApp.Components.Shared;
using ClientApp.Models;
using Employee.Module;
using Employee.Module.Business;
using EmployeeDocuments.Module;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddEmployee : AuthenticatedComponentBase
    {
        [Parameter][SupplyParameterFromQuery] public long? Id { get;  set; }

        // --- Injections ---
        [Inject] protected IEmployeeModule EmployeeModule { get; set; } = default!;
        [Inject] protected IEmployeeDocumentModule EmployeeDocumentModule { get; set; } = default!;

        // --- État du composant ---
        protected EmployeeViewModel employeeModel = new();
        protected EntrepriseViewModel? currentCompany;
        protected bool isLoadingCompany = true;
        protected bool isLoadingEmployee = false;
        protected string selectedDocumentType = "CV";
        protected List<AttachedDocumentViewModel> attachedDocuments = new();       
        protected bool isSubmitting = false;
        protected List<string> errors = new();
        protected string successMessage = string.Empty;
        protected List<Guid> documentsToDelete = new();

        // --- Cycle de vie ---
        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadCurrentCompany();

            if (Id.HasValue && currentCompany != null)
            {
                await LoadEmployee(Id.Value);
            }
        }

        #region Private
        private async Task LoadCurrentCompany()
        {
            isLoadingCompany = true;
            try
            {
                var company = await GetOrLoadCurrentCompanyAsync();
                if (company != null)
                {
                    currentCompany = EntrepriseViewModel.FromBusinessModel(company);
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error loading company data: {ex.Message}");
            }
            finally
            {
                isLoadingCompany = false;
            }
        }

        private async Task LoadEmployee(long employeeId)
        {
            isLoadingEmployee = true;
            try
            {
                var employees = await EmployeeModule.GetEmployeesByEnterpriseIdAsync(currentCompany!.Id);
                var emp = employees.FirstOrDefault(e => e.EmployeeID == employeeId);
                if (emp != null)
                {
                    employeeModel = new EmployeeViewModel
                    {
                        Nom = emp.Nom,
                        Prenom = emp.Prenom,
                        Age = emp.Age,
                        Sexe = emp.Sexe,
                        NomPoste = emp.NomPoste ?? string.Empty,
                        Fonctions = emp.Fonctions ?? string.Empty,
                        NombreMoisPoste = emp.NombreMoisPoste,
                        StatutEmploye = emp.StatutEmploye,
                        NumeroMatricule = emp.NumeroMatricule ?? string.Empty,
                        DateFinContrat = emp.DateFinContrat,
                        VouloirSouscrire = emp.VouloirSouscrire
                    };

                    // Load existing documents
                    var docs = await EmployeeDocumentModule.GetDocumentsByEmployeeIdAsync(employeeId);
                    attachedDocuments = docs.Select(d => new AttachedDocumentViewModel
                    {
                        ExistingDocument = d,
                        TypeDocument = d.TypeDocument ?? "Autre"
                    }).ToList();

                    documentsToDelete.Clear();
                }
                else
                {
                    errors.Add("Employee not found.");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Error loading employee data: {ex.Message}");
            }
            finally
            {
                isLoadingEmployee = false;
            }
        }

        private async Task HandleSubmit()
        {
            if (currentCompany == null) return;

            isSubmitting = true;
            errors.Clear();
            successMessage = string.Empty;

            try
            {
                var businessModel = new EmployeeBusinessModel
                {
                    EmployeeID = (Id ?? 0),
                    Nom = employeeModel.Nom,
                    Prenom = employeeModel.Prenom,
                    Age = employeeModel.Age,
                    Sexe = employeeModel.Sexe,
                    NomPoste = employeeModel.NomPoste,
                    Fonctions = employeeModel.Fonctions,
                    NombreMoisPoste = employeeModel.NombreMoisPoste,
                    StatutEmploye = employeeModel.StatutEmploye,
                    EntrepriseID = currentCompany.Id,
                    IsActive = true, // Maintain active status on update for now or handle specifically
                    NumeroMatricule = employeeModel.NumeroMatricule,
                    DateFinContrat = null, // Ensure it is nulled if saved as active
                    VouloirSouscrire = employeeModel.VouloirSouscrire
                };

                long employeeId;
                if (Id.HasValue)
                {
                    await EmployeeModule.SetEmployeeAsync(businessModel);
                    employeeId = Id.Value;
                    successMessage = $"Employee {employeeModel.Prenom} {employeeModel.Nom} updated successfully!";
                }
                else
                {
                    employeeId = await EmployeeModule.AddEmployeeAsync(businessModel);
                    successMessage = $"Employee {employeeModel.Prenom} {employeeModel.Nom} added successfully!";
                }

                if (employeeId > 0)
                {
                    // Process deletions
                    foreach (var streamId in documentsToDelete)
                    {
                        try
                        {
                            await EmployeeDocumentModule.RemoveDocumentAsync(employeeId, streamId);
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"Failed to delete document: {ex.Message}");
                        }
                    }

                    // Upload attached documents
                    foreach (var doc in attachedDocuments.Where(d => d.File != null)) // Only upload new files
                    {
                        try
                        {
                            using var stream = doc.File!.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
                            using var ms = new MemoryStream();
                            await stream.CopyToAsync(ms);
                            var fileContent = ms.ToArray();
                            await EmployeeDocumentModule.UploadAndLinkDocumentAsync(employeeId, doc.File.Name, fileContent, doc.TypeDocument);
                        }
                        catch (Exception docEx)
                        {
                            errors.Add($"Failed to upload {doc.File?.Name}: {docEx.Message}");
                        }
                    }

                    if (!Id.HasValue)
                    {
                        employeeModel = new EmployeeViewModel();
                        attachedDocuments.Clear();
                    }

                    // Optional: Hide success message and navigate back after some time
                    _ = Task.Delay(3000).ContinueWith(_ => {
                        InvokeAsync(() => {
                            successMessage = string.Empty;

                            StateHasChanged();
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                errors.Add($"Fail to save employee: {ex.Message}");
            }
            finally
            {
                isSubmitting = false;
            }
        }

        private void OnFileSelected(InputFileChangeEventArgs e)
        {
            foreach (var file in e.GetMultipleFiles(10))
            {
                attachedDocuments.Add(new AttachedDocumentViewModel
                {
                    File = file,
                    TypeDocument = selectedDocumentType
                });
            }
        }

        private void RemoveDocument(AttachedDocumentViewModel doc)
        {
            if (doc.ExistingDocument != null)
            {
                documentsToDelete.Add(doc.ExistingDocument.StreamId);
            }
            attachedDocuments.Remove(doc);
        }
        #endregion
    }
}
