namespace Company.Warehouse.Module.Data.Models
{
    public class EntrepriseWarehouseDataModel
    {
        public long Id { get; set; }
        public long EntrepriseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal SizeM2 { get; set; }
        public string? ContentsDescription { get; set; }
        public string? Address { get; set; }
        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }
        public string? PolicyNumber { get; set; }
    }
}
