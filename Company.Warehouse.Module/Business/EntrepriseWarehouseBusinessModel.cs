using Company.Warehouse.Module.Data.Models;

namespace Company.Warehouse.Module.Business
{
    public class EntrepriseWarehouseBusinessModel
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

        internal static EntrepriseWarehouseBusinessModel? FromDataModel(EntrepriseWarehouseDataModel? dataModel)
        {
            if (dataModel == null) return null;
            return new EntrepriseWarehouseBusinessModel
            {
                Id = dataModel.Id,
                EntrepriseId = dataModel.EntrepriseId,
                Name = dataModel.Name,
                SizeM2 = dataModel.SizeM2,
                ContentsDescription = dataModel.ContentsDescription,
                Address = dataModel.Address,
                WantsInsurance = dataModel.WantsInsurance,
                IsInsured = dataModel.IsInsured,
                PolicyNumber = dataModel.PolicyNumber
            };
        }

        internal EntrepriseWarehouseDataModel ToDataModel()
        {
            return new EntrepriseWarehouseDataModel
            {
                Id = Id,
                EntrepriseId = EntrepriseId,
                Name = Name,
                SizeM2 = SizeM2,
                ContentsDescription = ContentsDescription,
                Address = Address,
                WantsInsurance = WantsInsurance,
                IsInsured = IsInsured,
                PolicyNumber = PolicyNumber
            };
        }
    }
}
