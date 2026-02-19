namespace ClientApp.Models
{
    public class CompanySinisterViewModel
    {
        public long Id { get; set; }

        public string AssetType { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public long? EntrepriseFleetId { get; set; }

        public long EntrepriseId { get; set; }

        public long? EntrepriseMerchandiseTransportationId { get; set; }

        public long? EntrepriseWarehouseId { get; set; }

        public decimal EstimatedAmount { get; set; }

        public DateTime LastModifiedDate { get; set; }

        public decimal? ResolvedAmount { get; set; }

        public DateTime SinisterDate { get; set; } = DateTime.UtcNow;

        public long? SinisterId { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
