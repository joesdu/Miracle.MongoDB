using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Miracle.MongoDB.GridFS
{
    [ApiController]
    [Route("[controller]")]
    public class GridFSController : ControllerBase
    {
        private readonly GridFSBucket bucket;
        public GridFSController(GridFSBucket bucket) => this.bucket = bucket;

        /// <summary>
        /// 添加一个或多个文件
        /// </summary>
        /// <param name="dto">UploadGridFS</param>
        /// <returns></returns>
        [HttpPost("UploadMulti")]
        public async Task<IEnumerable<GridFSItem>> PostMulti([FromForm] UploadGridFSMulti fs)
        {
            if (string.IsNullOrWhiteSpace(fs.BusinessType)) throw new("BusinessType can not be null");
            if (fs.File is null || fs.File.Count == 0) throw new("no files find");
            if (fs.DeleteIds.Count > 0) foreach (var did in fs.DeleteIds) await bucket.DeleteAsync(ObjectId.Parse(did));
            var rsList = new List<GridFSItem>();
            foreach (var item in fs.File)
            {
                if (item.ContentType is null) throw new("ContentType in File is null");
                var upo = new GridFSUploadOptions
                {
                    BatchSize = fs.File.Count,
                    Metadata = new() { { "contentType", item.ContentType } }
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
        /// 添加一个或多个文件
        /// </summary>
        /// <param name="dto">UploadGridFS</param>
        /// <returns></returns>
        [HttpPost("UploadSingle")]
        public async Task<GridFSItem> PostSingle([FromForm] UploadGridFSSingle fs)
        {
            if (string.IsNullOrWhiteSpace(fs.BusinessType)) throw new("BusinessType can not be null");
            if (fs.File is null) throw new("no files find");
            if (!string.IsNullOrEmpty(fs.DeleteId)) await bucket.DeleteAsync(ObjectId.Parse(fs.DeleteId));
            if (fs.File.ContentType is null) throw new("ContentType in File is null");
            var bapp = fs.App ?? GridFSExtensions.BusinessApp;
            if (string.IsNullOrWhiteSpace(bapp)) throw new("BusinessApp can't be null");
            var metadata = new Dictionary<string, object> { { "app", bapp }, { "creator", new { fs.UserId, fs.UserName }.ToBsonDocument() } };
            if (!string.IsNullOrWhiteSpace(fs.BusinessType)) metadata.Add("business", fs.BusinessType);
            if (!string.IsNullOrWhiteSpace(fs.CategoryId)) metadata.Add("category", fs.CategoryId);
            var upo = new GridFSUploadOptions
            {
                BatchSize = 1,
                Metadata = new() { { "contentType", fs.File.ContentType } }
            };
            upo.Metadata.AddRange(metadata);
            var oid = await bucket.UploadFromStreamAsync(fs.File.FileName, fs.File.OpenReadStream(), upo);
            return new()
            {
                FileId = oid.ToString(),
                FileName = fs.File.FileName,
                Length = fs.File.Length,
                ContentType = fs.File.ContentType
            };
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        [HttpGet("Download/{id}")]
        public async Task<FileStreamResult> Download(string id)
        {
            var stream = await bucket.OpenDownloadStreamAsync(ObjectId.Parse(id), new() { Seekable = true });
            return File(stream, stream.FileInfo.Metadata["contentType"].AsString, stream.FileInfo.Filename);
        }

        /// <summary>
        /// 打开文件内容
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        [HttpGet("FileContent/{id}")]
        public async Task<FileContentResult> FileContent(string id)
        {
            var fi = await bucket.Find("{_id:ObjectId('" + id + "')}").SingleOrDefaultAsync() ?? throw new("no data find");
            var bytes = await bucket.DownloadAsBytesAsync(ObjectId.Parse(id), new GridFSDownloadOptions() { Seekable = true });
            return File(bytes, fi.Metadata["contentType"].AsString, fi.Filename);
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <param name="newname">新名称</param>
        /// <returns></returns>
        [HttpPut("{id}/Rename/{newname}")]
        public async Task Rename(string id, string newname) => await bucket.RenameAsync(ObjectId.Parse(id), newname);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task Delete(string id) => await bucket.DeleteAsync(ObjectId.Parse(id));
    }
}
