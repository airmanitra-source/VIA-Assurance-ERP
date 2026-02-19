using Company.Warehouse.Module.Data.Models;

namespace Company.Warehouse.Module.Business
{
    public class EntrepriseWarehouseMaterialBusinessModel
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal ApproximateValue { get; set; }
        public bool WantsInsurance { get; set; }
        public bool IsInsured { get; set; }

        internal static EntrepriseWarehouseMaterialBusinessModel? FromDataModel(EntrepriseWarehouseMaterialDataModel? dataModel)
        {
            if (dataModel == null) return null;

            return new EntrepriseWarehouseMaterialBusinessModel
            {
                Id = dataModel.Id,
                WarehouseId = dataModel.WarehouseId,
                Description = dataModel.Description,
                ApproximateValue = dataModel.ApproximateValue,
                WantsInsurance = dataModel.WantsInsurance,
                IsInsured = dataModel.IsInsured
            };
        }

        internal EntrepriseWarehouseMaterialDataModel ToDataModel()
        {
            return new EntrepriseWarehouseMaterialDataModel
            {
                Id = Id,
                WarehouseId = WarehouseId,
                Description = Description,
                ApproximateValue = ApproximateValue,
                WantsInsurance = WantsInsurance,
                IsInsured = IsInsured
            };
        }
    }
}
