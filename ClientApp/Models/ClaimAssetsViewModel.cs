using System.Collections.Generic;

namespace ClientApp.Models
{
    public class ClaimAssetsViewModel
    {
        public List<EntrepriseFleetViewModel> Fleets { get; set; } = new();
        public List<SinisterTypeViewModel> SinisterTypes { get; set; } = new();
        public List<TransportationViewModel> Transportations { get; set; } = new();
        public List<WarehouseViewModel> Warehouses { get; set; } = new();
    }
}
