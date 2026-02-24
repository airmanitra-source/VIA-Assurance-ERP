using Company.Sinister.Module.Data.Models;

namespace Company.Sinister.Module.Business
{
    public class CompanySinisterTypeBusinessModel
    {
        public long Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public long EntrepriseSinisterId { get; set; }

        public long SinisterTypeId { get; set; }

        public static CompanySinisterTypeBusinessModel From(CompanySinisterTypeDataModel dataModel)
        {
            return new CompanySinisterTypeBusinessModel
            {
                CreatedDate = dataModel.CreatedDate,
                EntrepriseSinisterId = dataModel.EntrepriseSinisterId,
                Id = dataModel.Id,
                SinisterTypeId = dataModel.SinisterTypeId
            };
        }

        public CompanySinisterTypeDataModel ToDataModel()
        {
            return new CompanySinisterTypeDataModel
            {
                CreatedDate = CreatedDate,
                EntrepriseSinisterId = EntrepriseSinisterId,
                Id = Id,
                SinisterTypeId = SinisterTypeId
            };
        }
    }
}
