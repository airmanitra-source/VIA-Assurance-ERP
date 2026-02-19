namespace FileTable.Infrastructure.FileTableDb.Entities
{
    public class EntrepriseWarehouseEntity
    {
        public long Id { get; set; }

        public string? Address { get; set; }

        public string? ContentsDescription { get; set; }

        public long EntrepriseId { get; set; }

        public bool IsInsured { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal SizeM2 { get; set; }

        public bool WantsInsurance { get; set; }
    }
}
