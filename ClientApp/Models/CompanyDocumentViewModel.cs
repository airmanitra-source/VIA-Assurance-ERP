namespace ClientApp.Models
{
    public class CompanyDocumentViewModel
    {
        public Guid StreamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? FileType { get; set; }
        public long? CachedFileSize { get; set; }
        public DateTimeOffset? CreationTime { get; set; }
        public DateTimeOffset? LastWriteTime { get; set; }
        public DateTimeOffset? LastAccessTime { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsOffline { get; set; }
        public bool IsHidden { get; set; }
        public bool IsReadonly { get; set; }
        public bool IsArchive { get; set; }
        public bool IsSystem { get; set; }
        public bool IsTemporary { get; set; }
        public string? TypeDocument { get; set; }
        public bool IsSigned { get; set; }
        public DateTime? SignedDate { get; set; }

        public string SizeDisplay => (CachedFileSize ?? 0) < 1024 ? $"{CachedFileSize ?? 0} B" :
                                     (CachedFileSize ?? 0) < 1048576 ? $"{(CachedFileSize ?? 0) / 1024.0:F1} KB" :
                                     $"{(CachedFileSize ?? 0) / 1048576.0:F1} MB";

        public List<CompanyDocumentViewModel> Children { get; set; } = new();
    }
}
