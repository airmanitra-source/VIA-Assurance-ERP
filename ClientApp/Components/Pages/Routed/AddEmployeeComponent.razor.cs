using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ClientApp.Components.Pages.Routed
{
    public partial class AddEmployeeComponent : AuthenticatedComponentBase
    {
        // --- Parameters (Id first per standards) ---
        [Parameter][SupplyParameterFromQuery] public long? Id { get; set; }

        // --- Injections (alphabetically sorted) ---
        [Inject] protected EmployeeController Controller { get; set; } = default!;

        // --- State (alphabetically sorted) ---
        protected List<AttachedDocumentViewModel> attachedDocuments = new();
        protected EntrepriseViewModel? currentCompany;
        protected List<Guid> documentsToDelete = new();
        protected EmployeeViewModel employeeModel = new();
        protected List<string> errors = new();
        protected bool isLoadingCompany = true;
        protected bool isLoadingEmployee = false;
        protected bool isSubmitting = false;
        protected List<ProjectViewModel> projects = new();
        protected string selectedDocumentType = "CV";
        protected string successMessage = string.Empty;

        // --- Lifecycle ---
        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadCurrentCompanyAsync();
            await LoadProjectsAsync();

            if (Id.HasValue && currentCompany != null)
            {
                await LoadEmployeeAsync();
            }
        }

        // --- Event Handlers (only state and events per standards) ---
        protected async Task HandleSubmitAsync()
        {
            if (currentCompany == null) return;

            isSubmitting = true;
            errors.Clear();
            successMessage = string.Empty;

            var result = await Controller.Store(
                employeeModel,
                Id,
                currentCompany.Id,
                attachedDocuments,
                documentsToDelete);

            errors = result.Errors;
            successMessage = result.Message;
            isSubmitting = false;

            if (result.Success && !Id.HasValue)
            {
                // Reset form for new entry
                employeeModel = new EmployeeViewModel();
                attachedDocuments.Clear();
            }

            if (result.Success)
            {
                _ = Task.Delay(3000).ContinueWith(_ =>
                {
                    InvokeAsync(() =>
                    {
                        successMessage = string.Empty;
                        StateHasChanged();
                    });
                });
            }
        }

        protected void OnFileSelected(InputFileChangeEventArgs e)
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

        protected void RemoveDocument(AttachedDocumentViewModel doc)
        {
            if (doc.ExistingDocument != null)
            {
                documentsToDelete.Add(doc.ExistingDocument.StreamId);
            }
            attachedDocuments.Remove(doc);
        }

        #region Private Helpers
        private async Task LoadProjectsAsync()
        {
            try
            {
                projects = await Controller.IndexProjectsAsync();
            }
            catch (Exception ex)
            {
                errors.Add($"Error loading projects: {ex.Message}");
            }
        }

        private async Task LoadCurrentCompanyAsync()
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

        private async Task LoadEmployeeAsync()
        {
            if (!Id.HasValue || currentCompany == null) return;

            isLoadingEmployee = true;
            try
            {
                var detail = await Controller.Show(Id.Value, currentCompany.Id);
                if (detail != null)
                {
                    employeeModel = detail.Employee;
                    attachedDocuments = detail.Documents;
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
        #endregion
    }
}
