namespace Miracle.MongoDB.GridFS;
public class GridFSItem
{
    public string FileId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public long Length { get; set; }
    public string ContentType { get; set; } = string.Empty;
}