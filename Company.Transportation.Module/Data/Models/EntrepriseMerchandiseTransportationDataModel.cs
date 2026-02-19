namespace Company.Transportation.Module.Data.Models
{
    public class EntrepriseMerchandiseTransportationDataModel
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
    }
}
