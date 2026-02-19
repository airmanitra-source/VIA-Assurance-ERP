namespace FileTable.Infrastructure.FileTableDb.Entities
{
    internal class CompanyDocumentEntity
    {
        public long ID { get; set; }
        public long EntrepriseID { get; set; }
        public Guid FileStreamID { get; set; }
        public string? TypeDocument { get; set; }
    }
}
