using Company.Warehouse.Module.Data.Models;

namespace Company.Warehouse.Module.Business
{
    public class EntrepriseWarehouseBusinessModel
    {
        public long Id { get; set; }

        public string? Address { get; set; }

        public string? ContentsDescription { get; set; }

        public long EntrepriseId { get; set; }

        public bool IsInsured { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? PolicyNumber { get; set; }

        public decimal SizeM2 { get; set; }

        public bool WantsInsurance { get; set; }

        internal static EntrepriseWarehouseBusinessModel? FromDataModel(EntrepriseWarehouseDataModel? dataModel)
        {
            if (dataModel == null) return null;
            return new EntrepriseWarehouseBusinessModel
            {
                Id = dataModel.Id,
                Address = dataModel.Address,
                ContentsDescription = dataModel.ContentsDescription,
                EntrepriseId = dataModel.EntrepriseId,
                IsInsured = dataModel.IsInsured,
                Name = dataModel.Name,
                PolicyNumber = dataModel.PolicyNumber,
                SizeM2 = dataModel.SizeM2,
                WantsInsurance = dataModel.WantsInsurance
            };
        }

        internal EntrepriseWarehouseDataModel ToDataModel()
        {
            return new EntrepriseWarehouseDataModel
            {
                Id = Id,
                Address = Address,
                ContentsDescription = ContentsDescription,
                EntrepriseId = EntrepriseId,
                IsInsured = IsInsured,
                Name = Name,
                PolicyNumber = PolicyNumber,
                SizeM2 = SizeM2,
                WantsInsurance = WantsInsurance
            };
        }
    }
}
