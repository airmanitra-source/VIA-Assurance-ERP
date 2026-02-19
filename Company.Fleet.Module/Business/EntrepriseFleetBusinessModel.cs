
using Company.Fleet.Module.Data.Models;

namespace Company.Fleet.Module.Business
{
    public class EntrepriseFleetBusinessModel
    {
        public long Id { get; set; }

        public long EntrepriseId { get; set; }

        public bool IsInsured { get; set; }

        public bool IsWorking { get; set; }

        public string Make { get; set; } = string.Empty;

        public int Mileage { get; set; }

        public string Model { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        public string Type { get; set; } = string.Empty; // 'Auto' or 'Moto'

        public bool WantsInsurance { get; set; }

        public int Year { get; set; }

        internal static EntrepriseFleetBusinessModel? FromDataModel(EntrepriseFleetDataModel? fleetDataModel)
        {
            if (fleetDataModel == null) return new();
            return new EntrepriseFleetBusinessModel
            {
                Id = fleetDataModel.Id,
                EntrepriseId = fleetDataModel.EntrepriseId,
                IsInsured = fleetDataModel.IsInsured,
                IsWorking = fleetDataModel.IsWorking,
                Make = fleetDataModel.Make,
                Mileage = fleetDataModel.Mileage,
                Model = fleetDataModel.Model,
                PolicyNumber = fleetDataModel.PolicyNumber,
                Type = fleetDataModel.Type,
                WantsInsurance = fleetDataModel.WantsInsurance,
                Year = fleetDataModel.Year
            };
        }

        internal EntrepriseFleetDataModel ToDataModel()
        {
            return new EntrepriseFleetDataModel
            {
                Id = Id,
                EntrepriseId = EntrepriseId,
                IsInsured = IsInsured,
                IsWorking = IsWorking,
                Make = Make,
                Mileage = Mileage,
                Model = Model,
                PolicyNumber = PolicyNumber,
                Type = Type,
                WantsInsurance = WantsInsurance,
                Year = Year
            };
        }
    }
}
