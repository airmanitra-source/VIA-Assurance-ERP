namespace CompanySinisterDocument.Module.Business
{
    public class CompanySinisterDocumentBusinessModel
    {
        public long? CachedFileSize { get; set; }

        public List<CompanySinisterDocumentBusinessModel> Children { get; set; } = new();

        public DateTimeOffset? CreationTime { get; set; }

        public string? FileType { get; set; }

        public bool IsArchive { get; set; }

        public bool IsDirectory { get; set; }

        public bool IsHidden { get; set; }

        public bool IsOffline { get; set; }

        public bool IsReadonly { get; set; }

        public bool IsSystem { get; set; }

        public bool IsTemporary { get; set; }

        public DateTimeOffset? LastAccessTime { get; set; }

        public DateTimeOffset? LastWriteTime { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? ParentPathLocator { get; set; }

        public string? PathLocator { get; set; }

        public Guid StreamId { get; set; }

        public string? TypeDocument { get; set; }
    }
}
