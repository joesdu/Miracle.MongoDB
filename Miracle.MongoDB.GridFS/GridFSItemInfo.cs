namespace Miracle.MongoDB.GridFS;
public class GridFSItemInfo
{
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long Length { get; set; }
    public string ContentType { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string App { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string? CategoryId { get; set; }
    public DateTime CreatTime { get; set; }
}