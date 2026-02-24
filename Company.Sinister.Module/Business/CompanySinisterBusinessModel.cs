using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module.Business
{
    public class CompanySinisterBusinessModel
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

        public DateTime SinisterDate { get; set; }

        public long? SinisterId { get; set; }

        public string Status { get; set; } = "Pending";

        public static CompanySinisterBusinessModel From(CompanySinisterDataModel dataModel)
        {
            return new CompanySinisterBusinessModel
            {
                AssetType = dataModel.AssetType,
                CreatedDate = dataModel.CreatedDate,
                Description = dataModel.Description,
                EntrepriseFleetId = dataModel.EntrepriseFleetId,
                EntrepriseId = dataModel.EntrepriseId,
                EntrepriseMerchandiseTransportationId = dataModel.EntrepriseMerchandiseTransportationId,
                EntrepriseWarehouseId = dataModel.EntrepriseWarehouseId,
                EstimatedAmount = dataModel.EstimatedAmount,
                Id = dataModel.Id,
                LastModifiedDate = dataModel.LastModifiedDate,
                ResolvedAmount = dataModel.ResolvedAmount,
                SinisterDate = dataModel.SinisterDate,
                SinisterId = dataModel.SinisterId,
                Status = dataModel.Status
            };
        }

        public CompanySinisterDataModel ToDataModel()
        {
            return new CompanySinisterDataModel
            {
                AssetType = AssetType,
                CreatedDate = CreatedDate,
                Description = Description,
                EntrepriseFleetId = EntrepriseFleetId,
                EntrepriseId = EntrepriseId,
                EntrepriseMerchandiseTransportationId = EntrepriseMerchandiseTransportationId,
                EntrepriseWarehouseId = EntrepriseWarehouseId,
                EstimatedAmount = EstimatedAmount,
                Id = Id,
                LastModifiedDate = LastModifiedDate,
                ResolvedAmount = ResolvedAmount,
                SinisterDate = SinisterDate,
                SinisterId = SinisterId,
                Status = Status
            };
        }
    }
}
