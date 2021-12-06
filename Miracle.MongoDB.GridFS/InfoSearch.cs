using Miracle.Common;

namespace Miracle.MongoDB.GridFS;
public class InfoSearch : KeywordPageInfo
{
    public string FileName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string App { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }
}