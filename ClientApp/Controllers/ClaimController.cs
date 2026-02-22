using ClientApp.Models;
using Company.Fleet.Module;
using Company.Module;
using Company.Transportation.Module;
using Company.Warehouse.Module;
using Microsoft.AspNetCore.Authorization;

namespace ClientApp.Controllers
{
    [Authorize(Roles = "developer")]
    public class ClaimController
    {
        private readonly ICompanyFleetModule _fleetModule;
        private readonly ICompanyTransportationModule _transportationModule;
        private readonly ICompanyWarehouseModule _warehouseModule;
        private readonly ICompanyModule _companyModule;

        public ClaimController(
            ICompanyFleetModule fleetModule,
            ICompanyTransportationModule transportationModule,
            ICompanyWarehouseModule warehouseModule,
            ICompanyModule companyModule)
        {
            _fleetModule = fleetModule;
            _transportationModule = transportationModule;
            _warehouseModule = warehouseModule;
            _companyModule = companyModule;
        }

        public async Task<ClaimAssetsViewModel> Index(long enterpriseId)
        {
            var fleets = await _fleetModule.GetCompanyFleetAsync(enterpriseId);
            var transportations = await _transportationModule.GetCompanyTransportationsAsync(enterpriseId);
            var warehouses = await _warehouseModule.GetCompanyWarehousesAsync(enterpriseId);

            return new ClaimAssetsViewModel
            {
                Fleets = fleets.Select(f => new EntrepriseFleetViewModel
                {
                    Id = f.Id,
                    EntrepriseId = f.EntrepriseId,
                    Type = f.Type,
                    Year = f.Year,
                    IsWorking = f.IsWorking,
                    Mileage = f.Mileage,
                    Make = f.Make,
                    Model = f.Model,
                    WantsInsurance = f.WantsInsurance,
                    IsInsured = f.IsInsured
                }).ToList(),
                Transportations = transportations.Select(t => new TransportationViewModel
                {
                    Id = t.Id,
                    EntrepriseId = t.EntrepriseId,
                    Description = t.Description,
                    Value = t.Value,
                    DepartureDate = t.DepartureDate,
                    ArrivalDate = t.ArrivalDate,
                    Origin = t.Origin,
                    Destination = t.Destination,
                    Frequency = t.Frequency ?? "OneTime",
                    WantsInsurance = t.WantsInsurance,
                    IsInsured = t.IsInsured
                }).ToList(),
                Warehouses = warehouses.Select(w => new WarehouseViewModel
                {
                    Id = w.Id,
                    EntrepriseId = w.EntrepriseId,
                    Name = w.Name,
                    SizeM2 = w.SizeM2,
                    ContentsDescription = w.ContentsDescription,
                    Address = w.Address,
                    WantsInsurance = w.WantsInsurance,
                    IsInsured = w.IsInsured
                }).ToList()
            };
        }

        public async Task<StoreResult> Store(CompanySinisterViewModel viewModel, long enterpriseId)
        {
            // Simulate saving
            await Task.Delay(500);
            return new StoreResult { Message = "Claim submitted successfully!", Success = true };
        }
    }
}
