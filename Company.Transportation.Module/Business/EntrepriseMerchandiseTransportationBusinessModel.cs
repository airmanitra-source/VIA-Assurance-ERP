using Company.Transportation.Module.Data.Models;

namespace Company.Transportation.Module.Business
{
    public class EntrepriseMerchandiseTransportationBusinessModel
    {
        public long Id { get; set; }
        public long EntrepriseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string Origin { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Frequency { get; set; } = "OneTime";
        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }
        public string? PolicyNumber { get; set; }

        internal static EntrepriseMerchandiseTransportationBusinessModel? FromDataModel(EntrepriseMerchandiseTransportationDataModel? dataModel)
        {
            if (dataModel == null) return null;
            return new EntrepriseMerchandiseTransportationBusinessModel
            {
                Id = dataModel.Id,
                EntrepriseId = dataModel.EntrepriseId,
                Description = dataModel.Description,
                Value = dataModel.Value,
                DepartureDate = dataModel.DepartureDate,
                ArrivalDate = dataModel.ArrivalDate,
                Origin = dataModel.Origin,
                Destination = dataModel.Destination,
                Frequency = dataModel.Frequency,
                WantsInsurance = dataModel.WantsInsurance,
                IsInsured = dataModel.IsInsured,
                PolicyNumber = dataModel.PolicyNumber
            };
        }

        internal EntrepriseMerchandiseTransportationDataModel ToDataModel()
        {
            return new EntrepriseMerchandiseTransportationDataModel
            {
                Id = Id,
                EntrepriseId = EntrepriseId,
                Description = Description,
                Value = Value,
                DepartureDate = DepartureDate,
                ArrivalDate = ArrivalDate,
                Origin = Origin,
                Destination = Destination,
                Frequency = Frequency,
                WantsInsurance = WantsInsurance,
                IsInsured = IsInsured,
                PolicyNumber = PolicyNumber
            };
        }
    }
}
