namespace MyApplication.Abstraction.Dtos.Storage;

public record StorageItemDto
{
    public Stream? Stream { get; set; }
    public string FileName { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public string Url { get; set; }
    public long FileSize { get; set; }
    public DateTime CreatedAt { get; set; }
}