using Microsoft.AspNetCore.Components;
using ClientApp.Components.Shared;
using ClientApp.Models;
using ClientApp.Controllers;

namespace ClientApp.Components.Pages.Routed
{
    public partial class EmployeesList: AuthenticatedComponentBase
    {
        [Inject] protected EmployeeController EmployeeController { get; set; } = default!;

        protected List<EmployeeViewModel>? employees;
        protected bool isLoading = true;
        protected bool showDeactivateModal = false;
        protected EmployeeViewModel? employeeToDeactivate;
        protected DateTime deactivationDate = DateTime.Today;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadDataAsync();
        }

        #region Private
        private async Task LoadDataAsync()
        {
            isLoading = true;
            try
            {
                if (CurrentEnterpriseId.HasValue)
                {
                    employees = await EmployeeController.Index(CurrentEnterpriseId.Value);
                }
            }
            catch (Exception ex)
            {
                // Error handling could be added here
                Console.WriteLine($"Error loading employees: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void EditEmployee(EmployeeViewModel employee)
        {
            // Navigation to edit page or open modal
            // For now, let's assume we use the add-employee page but maybe with an ID
            Navigation.NavigateTo($"/add-employee?id={employee.EmployeeID}");
        }

        private void ManageDocuments(EmployeeViewModel employee)
        {
            Navigation.NavigateTo($"/employee-documents?employeeId={employee.EmployeeID}");
        }

        private void ManageInsurance(EmployeeViewModel employee)
        {
            if (employee.VouloirSouscrire)
            {
                Navigation.NavigateTo($"/employee-subscriptions/{employee.EmployeeID}");
            }
        }

        private async Task ToggleActiveStatusAsync(EmployeeViewModel employee)
        {
            if (employee.IsActive)
            {
                employeeToDeactivate = employee;
                deactivationDate = DateTime.Today;
                showDeactivateModal = true;
            }
            else
            {
                try
                {
                    var success = await EmployeeController.SetActiveStatus(employee.EmployeeID, true, null);
                    if (success)
                    {
                        employee.IsActive = true;
                        employee.DateFinContrat = null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error activating status: {ex.Message}");
                }
            }
        }

        private void CloseModal()
        {
            showDeactivateModal = false;
            employeeToDeactivate = null;
        }

        private async Task ConfirmDeactivationAsync()
        {
            if (employeeToDeactivate == null) return;

            try
            {
                var success = await EmployeeController.SetActiveStatus(employeeToDeactivate.EmployeeID, false, deactivationDate);
                if (success)
                {
                    // Success! Refresh the list
                    await LoadDataAsync();
                    CloseModal();
                }
                else
                {
                    Console.WriteLine("Failed to deactivate employee");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating employee: {ex.Message}");
            }
        }
        #endregion
    }
}