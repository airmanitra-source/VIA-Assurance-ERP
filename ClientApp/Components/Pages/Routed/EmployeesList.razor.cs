using Microsoft.AspNetCore.Components;
using ClientApp.Components.Shared;
using ClientApp.Models;
using Employee.Module;

namespace ClientApp.Components.Pages.Routed
{
    public partial class EmployeesList: AuthenticatedComponentBase
    {
        [Inject] protected IEmployeeModule EmployeeModule{ get; set; } = default!;

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
                    var employeeBusinessModel = await EmployeeModule.GetEmployeesByEnterpriseIdAsync(CurrentEnterpriseId.Value);
                    employees = employeeBusinessModel.ToList().Select(EmployeeViewModel.FromBusinessModel).ToList();
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
                    await EmployeeModule.SetEmployeeActiveStatusAsync(employee.EmployeeID, true, null);
                    employee.IsActive = true;
                    employee.DateFinContrat = null;
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
                await EmployeeModule.SetEmployeeActiveStatusAsync(employeeToDeactivate.EmployeeID, false, deactivationDate);
                employeeToDeactivate.IsActive = false;
                employeeToDeactivate.DateFinContrat = deactivationDate;
                CloseModal();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deactivating status: {ex.Message}");
            }
        }
        #endregion
    }
}