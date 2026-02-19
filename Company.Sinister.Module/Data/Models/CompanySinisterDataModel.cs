namespace Company.Sinister.Module.Data.Models
{
    public class CompanySinisterDataModel
    {
        public long Id { get; set; }
        public long EntrepriseId { get; set; }
        public DateTime SinisterDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // 'Pending', 'InProgress', 'Resolved', 'Rejected'
        public decimal EstimatedAmount { get; set; }
        public decimal? ResolvedAmount { get; set; }
        public string AssetType { get; set; } = string.Empty; // 'Fleet', 'Transportation', 'Warehouse'
        public long? EntrepriseFleetId { get; set; }
        public long? EntrepriseMerchandiseTransportationId { get; set; }
        public long? EntrepriseWarehouseId { get; set; }
        public long? SinisterId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
