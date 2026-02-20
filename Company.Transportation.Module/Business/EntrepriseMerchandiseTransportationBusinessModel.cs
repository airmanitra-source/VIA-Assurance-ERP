using Company.Transportation.Module.Data.Models;

namespace Company.Transportation.Module.Business
{
    public class EntrepriseMerchandiseTransportationBusinessModel
    {
        public DateTime ArrivalDate { get; set; }

        public DateTime DepartureDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public long EntrepriseId { get; set; }

        public decimal? FranchiseAmount { get; set; }

        public decimal? FranchisePercentage { get; set; }

        public string FranchiseType { get; set; } = "Fixed";

        public string Frequency { get; set; } = "OneTime";

        public long Id { get; set; }

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

        public bool IsInsured { get; set; }

        public string Origin { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        public decimal Value { get; set; }

        public bool WantsInsurance { get; set; }

        internal static EntrepriseMerchandiseTransportationBusinessModel? FromDataModel(EntrepriseMerchandiseTransportationDataModel? dataModel)
        {
            if (dataModel == null) return null;
            return new EntrepriseMerchandiseTransportationBusinessModel
            {
                ArrivalDate = dataModel.ArrivalDate,
                DepartureDate = dataModel.DepartureDate,
                Description = dataModel.Description,
                Destination = dataModel.Destination,
                EntrepriseId = dataModel.EntrepriseId,
                FranchiseAmount = dataModel.FranchiseAmount,
                FranchisePercentage = dataModel.FranchisePercentage,
                FranchiseType = dataModel.FranchiseType,
                Frequency = dataModel.Frequency,
                Id = dataModel.Id,
                InsuranceEndDate = dataModel.InsuranceEndDate,
                InsuranceStartDate = dataModel.InsuranceStartDate,
                IsInsured = dataModel.IsInsured,
                Origin = dataModel.Origin,
                PolicyNumber = dataModel.PolicyNumber,
                Value = dataModel.Value,
                WantsInsurance = dataModel.WantsInsurance
            };
        }

        internal EntrepriseMerchandiseTransportationDataModel ToDataModel()
        {
            return new EntrepriseMerchandiseTransportationDataModel
            {
                ArrivalDate = ArrivalDate,
                DepartureDate = DepartureDate,
                Description = Description,
                Destination = Destination,
                EntrepriseId = EntrepriseId,
                FranchiseAmount = FranchiseAmount,
                FranchisePercentage = FranchisePercentage,
                FranchiseType = FranchiseType,
                Frequency = Frequency,
                Id = Id,
                InsuranceEndDate = InsuranceEndDate,
                InsuranceStartDate = InsuranceStartDate,
                IsInsured = IsInsured,
                Origin = Origin,
                PolicyNumber = PolicyNumber,
                Value = Value,
                WantsInsurance = WantsInsurance
            };
        }
    }
}
