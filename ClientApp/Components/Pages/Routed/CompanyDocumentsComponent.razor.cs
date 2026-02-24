using ClientApp.Components.Shared;
using ClientApp.Models;
using ClientApp.Services;
using ClientApp.Controllers;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace ClientApp.Components.Pages.Routed
{
    public partial class CompanyDocumentsComponent : AuthenticatedComponentBase
    {
        // --- Parameters ---

        // --- Injections ---
        [Inject] protected CompanyDocumentsController Controller { get; set; } = default!;

        // --- State (alphabetically sorted, ID first if any, but streamId is in viewmodel) ---
        protected string activeTab = "preview";
        protected List<CompanyDocumentViewModel> documents = new();
        protected bool isGenerating = false;
        protected bool isLoading = true;
        protected bool isSigning = false;
        protected long selectedEntrepriseId;
        protected CompanyDocumentViewModel? selectedNode;
        protected bool showSignaturePad = false;
        protected SignaturePad? signaturePad;

        protected override async Task OnInitializedAuthenticatedAsync()
        {
            var entrepriseId = CurrentEnterpriseId;
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
                documents = await Controller.Index(selectedEntrepriseId);
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
                var signerName = CurrentUserEmail ?? "System User";
                
                await Controller.Sign(selectedEntrepriseId, selectedNode.StreamId, signerName, signatureDataUrl);
                
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
    }
}
