using Company.Transportation.Module.Data.Models;

namespace Company.Transportation.Module.Business
{
    public class EntrepriseMerchandiseTransportationBusinessModel
    {
        public long Id { get; set; }

        public DateTime ArrivalDate { get; set; }

        public DateTime DepartureDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public long EntrepriseId { get; set; }

        public string Frequency { get; set; } = "OneTime";

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
                Id = dataModel.Id,
                ArrivalDate = dataModel.ArrivalDate,
                DepartureDate = dataModel.DepartureDate,
                Description = dataModel.Description,
                Destination = dataModel.Destination,
                EntrepriseId = dataModel.EntrepriseId,
                Frequency = dataModel.Frequency,
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
                Id = Id,
                ArrivalDate = ArrivalDate,
                DepartureDate = DepartureDate,
                Description = Description,
                Destination = Destination,
                EntrepriseId = EntrepriseId,
                Frequency = Frequency,
                IsInsured = IsInsured,
                Origin = Origin,
                PolicyNumber = PolicyNumber,
                Value = Value,
                WantsInsurance = WantsInsurance
            };
        }
    }
}
