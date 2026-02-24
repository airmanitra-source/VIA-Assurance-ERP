namespace FileTable.Infrastructure.FileTableDb.Entities
{
    public class EntrepriseSinisterTypeEntity
    {
        public long Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public long EntrepriseSinisterId { get; set; }

        public long SinisterTypeId { get; set; }
    }
}
