using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Miracle.MongoDB.GridFS
{
    public class GridFSSdk
    {
        private readonly GridFSBucket bucket;
        public GridFSSdk(GridFSBucket bucket) => this.bucket = bucket;

        /// <summary>
        /// 添加一个或多个文件
        /// </summary>
        /// <param name="dto">UploadGridFS</param>
        /// <returns></returns>
        public async Task<IEnumerable<GridFSItem>> Post(UploadGridFS fs)
        {
            if (string.IsNullOrWhiteSpace(fs.BusinessType)) throw new("BusinessType can not be null");
            if (fs.File is null || fs.File.Count == 0) throw new("no files find");
            var rsList = new List<GridFSItem> { };
            if (fs.DeleteIds.Count > 0) foreach (var did in fs.DeleteIds) await bucket.DeleteAsync(ObjectId.Parse(did));
            foreach (var item in fs.File)
            {
                if (item.ContentType is null) throw new("ContentType in File is null");
                var upo = new GridFSUploadOptions
                {
                    BatchSize = fs.File.Count,
                    Metadata = new()
                    {
                        { "contentType", item.ContentType }
                    }
                };
                var bapp = fs.App ?? GridFSExtensions.BusinessApp;
                if (string.IsNullOrWhiteSpace(bapp)) throw new("BusinessApp can't be null");
                _ = upo.Metadata.AddRange(new BsonDocument { { "app", bapp } });
                if (!string.IsNullOrWhiteSpace(fs.BusinessType)) _ = upo.Metadata.AddRange(new BsonDocument { { "business", fs.BusinessType } });
                if (!string.IsNullOrWhiteSpace(fs.CategoryId)) _ = upo.Metadata.AddRange(new BsonDocument { { "category", fs.CategoryId } });
                _ = upo.Metadata.AddRange(new BsonDocument { { "creator", new { fs.UserId, fs.UserName }.ToBsonDocument() } });
                var oid = await bucket.UploadFromStreamAsync(item.FileName, item.OpenReadStream(), upo);
                rsList.Add(new()
                {
                    FileId = oid.ToString(),
                    FileName = item.FileName,
                    Length = item.Length,
                    ContentType = item.ContentType
                });
            }
            return rsList;
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        public async Task<GridFSDownloadStream> Download(string id) => await bucket.OpenDownloadStreamAsync(ObjectId.Parse(id), new() { Seekable = true });

        /// <summary>
        /// 打开文件内容
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        public async Task<(GridFSFileInfo, byte[])> FileContent(string id)
        {
            var fi = await bucket.Find("{_id:ObjectId('" + id + "')}").SingleOrDefaultAsync() ?? throw new("no data find");
            var bytes = await bucket.DownloadAsBytesAsync(ObjectId.Parse(id), new GridFSDownloadOptions() { Seekable = true });
            return (fi, bytes);
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <param name="newname">新名称</param>
        /// <returns></returns>
        public async Task Rename(string id, string newname) => await bucket.RenameAsync(ObjectId.Parse(id), newname);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        public async Task Delete(string id) => await bucket.DeleteAsync(ObjectId.Parse(id));
    }
}
