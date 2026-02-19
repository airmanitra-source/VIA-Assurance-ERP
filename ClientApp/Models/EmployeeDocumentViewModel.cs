namespace ClientApp.Models;

public class EmployeeDocumentViewModel
{
    public Guid StreamId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? PathLocator { get; set; }
    public string? ParentPathLocator { get; set; }
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

    // UI properties
    public List<EmployeeDocumentViewModel> Children { get; set; } = new();
    public bool IsExpanded { get; set; }

    // Helper property for display
    public string DisplayName => IsDirectory ? $"📁 {Name}" : $"📄 {Name}";

    public string SizeDisplay => IsDirectory
        ? ""
        : CachedFileSize.HasValue
            ? FormatBytes(CachedFileSize.Value)
            : "";

    private static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}