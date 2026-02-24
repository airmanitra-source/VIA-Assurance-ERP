namespace FileTable.Infrastructure.FileTableDb.Entities
{
    internal class CompanySinisterDocumentEntity
    {
        public long Id { get; set; }

        public long CompanySinisterId { get; set; }

        public long EntrepriseId { get; set; }

        public Guid FileStreamId { get; set; }

        public string? TypeDocument { get; set; }
    }
}
