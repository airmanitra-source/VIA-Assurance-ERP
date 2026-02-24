using ClientApp.Models;
using Company.Sinister.Module;
using Company.Sinister.Module.Business;
using CompanySinisterDocument.Module;

namespace ClientApp.Controllers
{
    public class SinisterListController
    {
        private readonly ICompanySinisterDocumentModule _sinisterDocumentModule;
        private readonly ICompanySinisterModule _sinisterModule;

        public SinisterListController(ICompanySinisterModule sinisterModule, ICompanySinisterDocumentModule sinisterDocumentModule)
        {
            _sinisterModule = sinisterModule;
            _sinisterDocumentModule = sinisterDocumentModule;
        }

        public async Task<List<CompanySinisterViewModel>> IndexAsync(long entrepriseId)
        {
            var items = await _sinisterModule.GetCompanySinistersAsync(entrepriseId);
            var viewModels = await Task.WhenAll(items.Select(MapToViewModelAsync));
            return viewModels.OrderByDescending(s => s.CreatedDate).ToList();
        }

        public async Task<List<CompanySinisterViewModel>> IndexByStatusAsync(long entrepriseId, string status)
        {
            var items = await _sinisterModule.GetSinistersByStatusAsync(entrepriseId, status);
            var viewModels = await Task.WhenAll(items.Select(MapToViewModelAsync));
            return viewModels.OrderByDescending(s => s.CreatedDate).ToList();
        }

        public async Task<bool> StoreApprovalAsync(long id, bool approved, decimal? resolvedAmount)
        {
            var sinister = await _sinisterModule.GetSinisterByIdAsync(id);
            if (sinister == null) return false;

            sinister.Status = approved ? "Resolved" : "Rejected";
            sinister.ResolvedAmount = approved ? resolvedAmount : null;

            await _sinisterModule.SetSinisterAsync(sinister);
            return true;
        }

        private async Task<CompanySinisterViewModel> MapToViewModelAsync(CompanySinisterBusinessModel m)
        {
            var viewModel = MapToViewModel(m);
            viewModel.AccidentPhotoDataUrls = await _sinisterDocumentModule.GetAccidentPhotoDataUrlsAsync(m.Id);
            return viewModel;
        }

        private static CompanySinisterViewModel MapToViewModel(CompanySinisterBusinessModel m)
        {
            return new CompanySinisterViewModel
            {
                AssetType = m.AssetType,
                CreatedDate = m.CreatedDate,
                Description = m.Description,
                EntrepriseFleetId = m.EntrepriseFleetId,
                EntrepriseId = m.EntrepriseId,
                EntrepriseMerchandiseTransportationId = m.EntrepriseMerchandiseTransportationId,
                EntrepriseWarehouseId = m.EntrepriseWarehouseId,
                EstimatedAmount = m.EstimatedAmount,
                Id = m.Id,
                LastModifiedDate = m.LastModifiedDate,
                ResolvedAmount = m.ResolvedAmount,
                SinisterDate = m.SinisterDate,
                SinisterId = m.SinisterId,
                Status = m.Status
            };
        }
    }
}
