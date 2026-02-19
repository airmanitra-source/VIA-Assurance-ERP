namespace FileTable.Infrastructure.FileTableDb.Entities
{
    internal class EmployeeDocumentEntity
    {
        public long Id { get; set; }
        public int EmployeeID { get; set; }
        public Guid FileStreamID { get; set; }
        public string? TypeDocument { get; set; }
    }
}
