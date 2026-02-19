namespace FileTable.Infrastructure.FileTableDb.Entities
{
    public class EntrepriseWarehouseMaterialEntity
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal ApproximateValue { get; set; }
        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }
    }
}
