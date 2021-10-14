using Microsoft.AspNetCore.Mvc;
using Miracle.MongoDB.GridFS;
using MongoDB.Driver.GridFS;

namespace example.api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GridFSController : ControllerBase
    {
        private readonly GridFSSdk gridSdk;
        public GridFSController(GridFSBucket bucket) => gridSdk = new(bucket);

        [HttpPost("Upload")]
        public async Task<IEnumerable<GridFSItem>> PostUnAuthorize([FromForm] UploadGridFS fs) => await gridSdk.Post(fs);

        [HttpGet("Download/{id}")]
        public async Task<FileStreamResult> Download(string id)
        {
            var stream = await gridSdk.Download(id);
            return File(stream, stream.FileInfo.Metadata["contentType"].AsString, stream.FileInfo.Filename);
        }

        [HttpGet("FileContent/{id}")]
        public async Task<FileContentResult> FileContent(string id)
        {
            var (fi, bytes) = await gridSdk.FileContent(id);
            return File(bytes, fi.Metadata["contentType"].AsString, fi.Filename);
        }

        [HttpPut("{id}/Rename/{newname}")]
        public async Task Rename(string id, string newname) => await gridSdk.Rename(id, newname);

        [HttpDelete("{id}")]
        public async Task Delete(string id) => await gridSdk.Delete(id);
    }
}
