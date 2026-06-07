using Company.Fleet.Module.Data.Models;

namespace Company.Fleet.Module.Business.Models
{
    public class EntrepriseFleetBusinessModel
    {
        public long EntrepriseId { get; set; }

        public decimal? FranchiseAmount { get; set; }

        public decimal? FranchisePercentage { get; set; }

        public string FranchiseType { get; set; } = "Fixed";

        public int? FiscalPower { get; set; }

        public long Id { get; set; }

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsInsured { get; set; }

        public bool IsWorking { get; set; }

        public string? LicensePlate { get; set; }

        public string Make { get; set; } = string.Empty;

        public int Mileage { get; set; }

        public string Model { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        public int? Power { get; set; }

        public string Type { get; set; } = string.Empty; // 'Auto' or 'Moto'

        public string? VIN { get; set; }

        public bool WantsInsurance { get; set; }

        public int Year { get; set; }

        internal static EntrepriseFleetBusinessModel? FromDataModel(EntrepriseFleetDataModel? fleetDataModel)
        {
            if (fleetDataModel == null) return new();
            return new EntrepriseFleetBusinessModel
            {
                EntrepriseId = fleetDataModel.EntrepriseId,
                FranchiseAmount = fleetDataModel.FranchiseAmount,
                FranchisePercentage = fleetDataModel.FranchisePercentage,
                FranchiseType = fleetDataModel.FranchiseType,
                FiscalPower = fleetDataModel.FiscalPower,
                Id = fleetDataModel.Id,
                InsuranceEndDate = fleetDataModel.InsuranceEndDate,
                InsuranceStartDate = fleetDataModel.InsuranceStartDate,
                IsInsured = fleetDataModel.IsInsured,
                IsWorking = fleetDataModel.IsWorking,
                LicensePlate = fleetDataModel.LicensePlate,
                Make = fleetDataModel.Make,
                Mileage = fleetDataModel.Mileage,
                Model = fleetDataModel.Model,
                PolicyNumber = fleetDataModel.PolicyNumber,
                Power = fleetDataModel.Power,
                Type = fleetDataModel.Type,
                VIN = fleetDataModel.VIN,
                WantsInsurance = fleetDataModel.WantsInsurance,
                Year = fleetDataModel.Year
            };
        }

        internal EntrepriseFleetDataModel ToDataModel()
        {
            return new EntrepriseFleetDataModel
            {
                EntrepriseId = EntrepriseId,
                FranchiseAmount = FranchiseAmount,
                FranchisePercentage = FranchisePercentage,
                FranchiseType = FranchiseType,
                FiscalPower = FiscalPower,
                Id = Id,
                InsuranceEndDate = InsuranceEndDate,
                InsuranceStartDate = InsuranceStartDate,
                IsInsured = IsInsured,
                IsWorking = IsWorking,
                LicensePlate = LicensePlate,
                Make = Make,
                Mileage = Mileage,
                Model = Model,
                PolicyNumber = PolicyNumber,
                Power = Power,
                Type = Type,
                VIN = VIN,
                WantsInsurance = WantsInsurance,
                Year = Year
            };
        }
    }
}
