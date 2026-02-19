
using Company.Fleet.Module.Data.Models;

namespace Company.Fleet.Module.Business
{
    public class EntrepriseFleetBusinessModel
    {
        public long Id { get; set; }
        public long EntrepriseId { get; set; }
        public string Type { get; set; } = string.Empty; // 'Auto' or 'Moto'
        public int Year { get; set; }
        public bool IsWorking { get; set; }
        public int Mileage { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }
        public string? PolicyNumber { get; set; }

        internal static EntrepriseFleetBusinessModel? FromDataModel(EntrepriseFleetDataModel? fleetDataModel)
        {
            if (fleetDataModel == null) return new();
            return new EntrepriseFleetBusinessModel
            {
                Id = fleetDataModel.Id,
                EntrepriseId = fleetDataModel.EntrepriseId,
                Type = fleetDataModel.Type,
                Year = fleetDataModel.Year,
                IsWorking = fleetDataModel.IsWorking,
                Mileage = fleetDataModel.Mileage,
                Make = fleetDataModel.Make,
                Model = fleetDataModel.Model,
                WantsInsurance = fleetDataModel.WantsInsurance,
                IsInsured = fleetDataModel.IsInsured,
                PolicyNumber = fleetDataModel.PolicyNumber
            };
        }

        internal EntrepriseFleetDataModel ToDataModel()
        {
            return new EntrepriseFleetDataModel
            {
                Id = Id,
                EntrepriseId = EntrepriseId,
                Type = Type,
                Year = Year,
                IsWorking = IsWorking,
                Mileage = Mileage,
                Make = Make,
                Model = Model,
                WantsInsurance = WantsInsurance,
                IsInsured = IsInsured,
                PolicyNumber = PolicyNumber
            };
        }
    }
}
