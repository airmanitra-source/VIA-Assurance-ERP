using ClientApp.Models;
using ClientApp.Services;
using EmployeeDocuments.Module;
using EmployeeDocuments.Module.Business;
using Employee.Module;
using Microsoft.AspNetCore.Components;

namespace ClientApp.Components.Pages.Routed
{
    public partial class EmployeeDocuments: ComponentBase
    {

        [Inject] protected IEmployeeDocumentModule _filetableModule { get; set; } = default!;
        [Inject] protected IEmployeeModule EmployeeModule { get; set; } = default!;
        [Inject] protected AuthenticationService AuthService{ get; set; } = default!;
        [Inject] protected NavigationManager Navigation{ get; set; } = default!;

        [Parameter][SupplyParameterFromQuery] public long? EmployeeId { get; set; }

        protected List<EmployeeDocumentViewModel>? documentTree;
        protected EmployeeDocumentViewModel? selectedNode;
        protected bool isLoading = true;
        protected string activeTab = "preview";

        protected override async Task OnInitializedAsync()
        {
            // Check authentication
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            await LoadDocumentsAsync();
        }

        #region Private
        private async Task HandleLogoutAsync()
        {
            await AuthService.LogoutAsync();
            Navigation.NavigateTo("/login", forceLoad: true);
        }

        private async Task LoadDocumentsAsync()
        {
            isLoading = true;
            try
            {
                var currentEntrepriseId = AuthService.GetCurrentEntrepriseId();
                List<EmployeeDocumentBusinessModel> businessModels;
                if (EmployeeId.HasValue)
                {
                    // Security check: verify employee belongs to this enterprise
                    var employee = await EmployeeModule.GetEmployeeByIdAsync((int)EmployeeId.Value);
                    if (employee == null || employee.EntrepriseID != currentEntrepriseId)
                    {
                        Navigation.NavigateTo("/list-employees");
                        return;
                    }
                    businessModels = await _filetableModule.GetDocumentsByEmployeeIdAsync(EmployeeId.Value);
                }
                else
                {
                    businessModels = await _filetableModule.GetAllDocumentsAsync();
                }
                var allDocuments = businessModels.Select(MapToModel).ToList();
                documentTree = BuildTree(allDocuments);
            }
            catch (Exception ex)
            {
                // Handle error - you might want to show a toast notification
                Console.WriteLine($"Error loading documents: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private EmployeeDocumentViewModel MapToModel(EmployeeDocumentBusinessModel businessModel)
        {
            return new EmployeeDocumentViewModel
            {
                StreamId = businessModel.StreamId,
                Name = businessModel.Name,
                PathLocator = businessModel.PathLocator,
                ParentPathLocator = businessModel.ParentPathLocator,
                FileType = businessModel.FileType,
                CachedFileSize = businessModel.CachedFileSize,
                CreationTime = businessModel.CreationTime,
                LastWriteTime = businessModel.LastWriteTime,
                LastAccessTime = businessModel.LastAccessTime,
                IsDirectory = businessModel.IsDirectory,
                IsOffline = businessModel.IsOffline,
                IsHidden = businessModel.IsHidden,
                IsReadonly = businessModel.IsReadonly,
                IsArchive = businessModel.IsArchive,
                IsSystem = businessModel.IsSystem,
                IsTemporary = businessModel.IsTemporary
            };
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