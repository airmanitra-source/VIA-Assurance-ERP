using ClientApp.Components.Shared;
using ClientApp.Models;
using ClientApp.Services;
using CompanyDocuments.Module;
using CompanyDocuments.Module.Business;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace ClientApp.Components.Pages.Routed
{
    public partial class CompanyDocuments : ComponentBase
    {
        [Inject] protected ICompanyDocumentModule DocumentModule { get; set; } = default!;
        [Inject] protected AuthenticationService AuthService { get; set; } = default!;
        [Inject] protected NavigationManager Navigation { get; set; } = default!;

        protected List<CompanyDocumentViewModel> documents = new();
        protected CompanyDocumentViewModel? selectedNode;
        protected SignaturePad? signaturePad;
        protected string activeTab = "preview";
        protected bool isLoading = true;
        protected bool isGenerating = false;
        protected bool isSigning = false;
        protected bool showSignaturePad = false;
        protected long selectedEntrepriseId;

        protected override async Task OnInitializedAsync()
        {
            if (!AuthService.IsAuthenticated())
            {
                Navigation.NavigateTo("/login");
                return;
            }

            var entrepriseId = AuthService.GetCurrentEntrepriseId();
            if (entrepriseId.HasValue)
            {
                selectedEntrepriseId = entrepriseId.Value;
                await LoadDocumentsAsync();
            }
            else
            {
                isLoading = false;
            }
        }

        protected async Task LoadDocumentsAsync()
        {
            isLoading = true;
            try
            {
                var businessDocs = await DocumentModule.GetDocumentsByEntrepriseIdAsync(selectedEntrepriseId);
                documents = businessDocs.Select(MapToViewModel).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading documents: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        protected void SelectNode(CompanyDocumentViewModel node)
        {
            selectedNode = node;
            activeTab = "preview";
        }

        protected async Task SignDocumentAsync()
        {
            if (selectedNode == null || selectedNode.IsDirectory || selectedNode.IsSigned) return;
            showSignaturePad = true;
        }

        protected async Task ConfirmSignatureAsync()
        {
            if (selectedNode == null || signaturePad == null) return;

            isSigning = true;
            try
            {
                var signatureDataUrl = await signaturePad.GetSignatureDataUrlAsync();
                var signerName = AuthService.GetCurrentUserEmail() ?? "System User";
                
                await DocumentModule.SignDocumentAsync(selectedEntrepriseId, selectedNode.StreamId, signerName, signatureDataUrl);
                
                showSignaturePad = false;
                await LoadDocumentsAsync();
                
                // Refresh selection
                selectedNode = documents.FirstOrDefault(d => d.StreamId == selectedNode.StreamId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error signing document: {ex.Message}");
            }
            finally
            {
                isSigning = false;
            }
        }

        private CompanyDocumentViewModel MapToViewModel(CompanyDocumentBusinessModel m)
        {
            return new CompanyDocumentViewModel
            {
                StreamId = m.StreamId,
                Name = m.Name,
                FileType = m.FileType,
                CachedFileSize = m.CachedFileSize,
                CreationTime = m.CreationTime,
                LastWriteTime = m.LastWriteTime,
                IsDirectory = m.IsDirectory,
                TypeDocument = m.TypeDocument,
                IsSigned = m.IsSigned,
                SignedDate = m.SignedDate
            };
        }
    }
}
