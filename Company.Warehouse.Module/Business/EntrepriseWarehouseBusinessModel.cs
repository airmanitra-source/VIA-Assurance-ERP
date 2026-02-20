using Company.Warehouse.Module.Data.Models;

namespace Company.Warehouse.Module.Business
{
    public class EntrepriseWarehouseBusinessModel
    {
        public string? Address { get; set; }

        public string? ContentsDescription { get; set; }

        public long EntrepriseId { get; set; }

        public decimal? FranchiseAmount { get; set; }

        public decimal? FranchisePercentage { get; set; }

        public string FranchiseType { get; set; } = "Fixed";

        public long Id { get; set; }

        public DateTime? InsuranceEndDate { get; set; }

        public DateTime? InsuranceStartDate { get; set; }

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
                Address = dataModel.Address,
                ContentsDescription = dataModel.ContentsDescription,
                EntrepriseId = dataModel.EntrepriseId,
                FranchiseAmount = dataModel.FranchiseAmount,
                FranchisePercentage = dataModel.FranchisePercentage,
                FranchiseType = dataModel.FranchiseType,
                Id = dataModel.Id,
                InsuranceEndDate = dataModel.InsuranceEndDate,
                InsuranceStartDate = dataModel.InsuranceStartDate,
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
                Address = Address,
                ContentsDescription = ContentsDescription,
                EntrepriseId = EntrepriseId,
                FranchiseAmount = FranchiseAmount,
                FranchisePercentage = FranchisePercentage,
                FranchiseType = FranchiseType,
                Id = Id,
                InsuranceEndDate = InsuranceEndDate,
                InsuranceStartDate = InsuranceStartDate,
                IsInsured = IsInsured,
                Name = Name,
                PolicyNumber = PolicyNumber,
                SizeM2 = SizeM2,
                WantsInsurance = WantsInsurance
            };
        }
    }
}
