using ClientApp.Components.Shared;
using ClientApp.Controllers;
using ClientApp.Models;
using Employee.Module;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class EmployeeDocuments : AuthenticatedComponentBase
    {
        // --- Parameters ---
        [Parameter][SupplyParameterFromQuery] public long? EmployeeId { get; set; }

        // --- Injections ---
        [Inject] protected EmployeeDocumentsController Controller { get; set; } = default!;
        [Inject] protected EmployeeController EmployeeController { get; set; } = default!;

        // --- State (alphabetically sorted, ID first if any) ---
        protected string activeTab = "preview";
        protected List<EmployeeDocumentViewModel>? documentTree;
        protected bool isLoading = true;
        protected EmployeeDocumentViewModel? selectedNode;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            await LoadDocumentsAsync();
        }

        #region Private
        protected async Task LoadDocumentsAsync()
        {
            isLoading = true;
            try
            {
                var currentEntrepriseId = CurrentEnterpriseId;
                List<EmployeeDocumentViewModel> allDocuments;

                if (EmployeeId.HasValue)
                {
                    // Security check: verify employee belongs to this enterprise
                    var employeeDetail = await EmployeeController.Show(EmployeeId.Value, currentEntrepriseId ?? 0);
                    if (employeeDetail == null || employeeDetail.Employee.EntrepriseId != currentEntrepriseId)
                    {
                        Navigation.NavigateTo("/list-employees");
                        return;
                    }
                    allDocuments = await Controller.Index(EmployeeId.Value);
                }
                else
                {
                    allDocuments = await Controller.Index();
                }

                documentTree = BuildTree(allDocuments);
            }
            catch (Exception ex)
            {
                // Handle error
                Console.WriteLine($"Error loading documents: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private List<EmployeeDocumentViewModel> BuildTree(List<EmployeeDocumentViewModel> documents)
        {
            var documentDict = documents.ToDictionary(d => d.PathLocator ?? string.Empty, d => d);
            var rootNodes = new List<EmployeeDocumentViewModel>();

            foreach (var doc in documents)
            {
                if (string.IsNullOrEmpty(doc.ParentPathLocator))
                {
                    // Root node
                    rootNodes.Add(doc);
                }
                else
                {
                    // Find parent and add as child
                    if (documentDict.TryGetValue(doc.ParentPathLocator, out var parent))
                    {
                        parent.Children.Add(doc);
                    }
                    else
                    {
                        // Orphaned node, add to root
                        rootNodes.Add(doc);
                    }
                }
            }

            return rootNodes;
        }

        private async Task RefreshTreeAsync()
        {
            selectedNode = null;
            await LoadDocumentsAsync();
        }

        private void HandleNodeSelected(EmployeeDocumentViewModel node)
        {
            selectedNode = node;
            activeTab = "preview"; // Switch to preview tab when selecting a new file
            StateHasChanged();
        }
        #endregion
    }
}