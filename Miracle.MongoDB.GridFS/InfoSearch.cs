using Miracle.Common;

namespace Miracle.MongoDB.GridFS
{
    public class InfoSearch: KeywordPageInfo
    {
        public string FileName { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }
        public  string App { get; set; }
        public string BusinessType { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
