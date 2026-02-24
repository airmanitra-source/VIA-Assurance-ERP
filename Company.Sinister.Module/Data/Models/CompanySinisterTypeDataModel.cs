namespace Company.Sinister.Module.Data.Models
{
    public class CompanySinisterTypeDataModel
    {
        public long Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public long EntrepriseSinisterId { get; set; }
        public long SinisterTypeId { get; set; }
    }
}
