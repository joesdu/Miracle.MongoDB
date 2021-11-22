namespace Miracle.MongoDB.GridFS;
public class GridFSItemInfo
{
    public string FileId { get; set; }
    public string FileName { get; set; }
    public long Length { get; set; }
    public string ContentType { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string App { get; set; }
    public string BusinessType { get; set; }
    public string CategoryId { get; set; }
    public DateTime CreatTime { get; set; }
}