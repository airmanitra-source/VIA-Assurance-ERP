using ClientApp.Models;
using Company.Sinister.Module;
using Company.Sinister.Module.Data.Models;

namespace ClientApp.Controllers
{
    public class SinisterListController
    {
        private readonly ICompanySinisterModule _sinisterModule;

        public SinisterListController(ICompanySinisterModule sinisterModule)
        {
            _sinisterModule = sinisterModule;
        }

        public async Task<List<CompanySinisterViewModel>> IndexAsync(long entrepriseId)
        {
            var items = await _sinisterModule.GetCompanySinistersAsync(entrepriseId);
            return items.Select(MapToViewModel).OrderByDescending(s => s.CreatedDate).ToList();
        }

        public async Task<List<CompanySinisterViewModel>> IndexByStatusAsync(long entrepriseId, string status)
        {
            var items = await _sinisterModule.GetSinistersByStatusAsync(entrepriseId, status);
            return items.Select(MapToViewModel).OrderByDescending(s => s.CreatedDate).ToList();
        }

        public async Task<bool> StoreApprovalAsync(long id, bool approved, decimal? resolvedAmount)
        {
            var sinister = await _sinisterModule.GetSinisterByIdAsync(id);
            if (sinister == null) return false;

            sinister.Status = approved ? "Resolved" : "Rejected";
            sinister.ResolvedAmount = approved ? resolvedAmount : null;

            await _sinisterModule.UpdateSinisterAsync(sinister);
            return true;
        }

        private static CompanySinisterViewModel MapToViewModel(CompanySinisterDataModel m)
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
