using Microsoft.AspNetCore.Mvc;
using Miracle.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Miracle.MongoDB.GridFS;
[ApiController]
[Route("[controller]")]
public class GridFSController : ControllerBase
{
    private readonly GridFSBucket Bucket;
    private readonly IMongoCollection<GridFSItemInfo> Coll;
    private readonly FilterDefinitionBuilder<GridFSItemInfo> _bf = Builders<GridFSItemInfo>.Filter;
    private readonly MiracleStaticFileSettings FileSetting;
    public GridFSController(GridFSBucket bucket, IMongoCollection<GridFSItemInfo> collection, IConfiguration config)
    {
        Bucket = bucket;
        Coll = collection;
        FileSetting = config.GetSection(MiracleStaticFileSettings.Postion).Get<MiracleStaticFileSettings>();
    }

    /// <summary>
    /// 获取已上传文件列表
    /// </summary>
    /// <param name="info">关键字支持:文件名,用户名,用户ID,App名称以及业务名称模糊匹配</param>
    /// <returns></returns>
    [HttpPost("Infos")]
    public async Task<object> Infos(InfoSearch info)
    {
        var f = _bf.Empty;
        if (!string.IsNullOrWhiteSpace(info.FileName)) f &= _bf.Where(c => c.FileName.Contains(info.FileName));
        if (!string.IsNullOrWhiteSpace(info.UserName)) f &= _bf.Where(c => c.UserName.Contains(info.UserName));
        if (!string.IsNullOrWhiteSpace(info.UserId)) f &= _bf.Where(c => c.UserId.Contains(info.UserId));
        if (!string.IsNullOrWhiteSpace(info.App)) f &= _bf.Where(c => c.App.Contains(info.App));
        if (!string.IsNullOrWhiteSpace(info.BusinessType)) f &= _bf.Where(c => c.BusinessType.Contains(info.BusinessType));
        if (info.Start is not null) f &= _bf.Gte(c => c.CreatTime, info.Start);
        if (info.End is not null) f &= _bf.Lte(c => c.CreatTime, info.End);
        if (!string.IsNullOrWhiteSpace(info.SearchKey)) f &= _bf.Or(_bf.Where(c => c.FileName.Contains(info.SearchKey)),
            _bf.Where(c => c.UserName.Contains(info.SearchKey)),
            _bf.Where(c => c.UserId.Contains(info.SearchKey)),
            _bf.Where(c => c.App.Contains(info.SearchKey)),
            _bf.Where(c => c.BusinessType.Contains(info.SearchKey)));
        var total = await Coll.CountDocumentsAsync(f);
        var list = await Coll.FindAsync(f, new()
        {
            Sort = Builders<GridFSItemInfo>.Sort.Descending(c => c.CreatTime),
            Limit = info.PageSize,
            Skip = (info.PageIndex - 1) * info.PageSize
        }).Result.ToListAsync();
        return PageResult.Wrap(total, list);
    }

    /// <summary>
    /// 添加一个或多个文件
    /// </summary>
    [HttpPost("UploadMulti")]
    public async Task<IEnumerable<GridFSItem>> PostMulti([FromForm] UploadGridFSMulti fs)
    {
        if (fs.File is null || fs.File.Count == 0) throw new("no files find");
        if (fs.DeleteIds.Count > 0) await Delete(fs.DeleteIds.ToArray());
        var rsList = new List<GridFSItem>();
        var infos = new List<GridFSItemInfo>();
        foreach (var item in fs.File)
        {
            if (item.ContentType is null) throw new("ContentType in File is null");
            var bapp = !string.IsNullOrWhiteSpace(fs.App) ? fs.App : GridFSExtensions.BusinessApp;
            if (string.IsNullOrWhiteSpace(bapp)) throw new("BusinessApp can't be null");
            var metadata = new Dictionary<string, object> { { "contentType", item.ContentType }, { "app", bapp }, { "creator", new { fs.UserId, fs.UserName }.ToBsonDocument() } };
            if (!string.IsNullOrWhiteSpace(fs.BusinessType)) metadata.Add("business", fs.BusinessType);
            if (!string.IsNullOrWhiteSpace(fs.CategoryId)) metadata.Add("category", fs.CategoryId);
            var upo = new GridFSUploadOptions
            {
                BatchSize = fs.File.Count,
                Metadata = new(metadata)
            };
            var oid = await Bucket.UploadFromStreamAsync(item.FileName, item.OpenReadStream(), upo);
            rsList.Add(new()
            {
                FileId = oid.ToString(),
                FileName = item.FileName,
                Length = item.Length,
                ContentType = item.ContentType
            });
            infos.Add(new()
            {
                FileId = oid.ToString(),
                FileName = item.FileName,
                Length = item.Length,
                ContentType = item.ContentType,
                UserId = fs.UserId,
                UserName = fs.UserName,
                App = fs.App,
                BusinessType = fs.BusinessType,
                CategoryId = fs.CategoryId,
                CreatTime = DateTime.Now
            });
        }
        _ = Coll.InsertManyAsync(infos);
        return rsList;
    }

    /// <summary>
    /// 添加一个或多个文件
    /// </summary>
    [HttpPost("UploadSingle")]
    public async Task<GridFSItem> PostSingle([FromForm] UploadGridFSSingle fs)
    {
        if (fs.File is null) throw new("no files find");
        if (!string.IsNullOrWhiteSpace(fs.DeleteId)) await Delete(fs.DeleteId);
        if (fs.File.ContentType is null) throw new("ContentType in File is null");
        var bapp = !string.IsNullOrWhiteSpace(fs.App) ? fs.App : GridFSExtensions.BusinessApp;
        if (string.IsNullOrWhiteSpace(bapp)) throw new("BusinessApp can't be null");
        var metadata = new Dictionary<string, object> { { "contentType", fs.File.ContentType }, { "app", bapp }, { "creator", new { fs.UserId, fs.UserName }.ToBsonDocument() } };
        if (!string.IsNullOrWhiteSpace(fs.BusinessType)) metadata.Add("business", fs.BusinessType);
        if (!string.IsNullOrWhiteSpace(fs.CategoryId)) metadata.Add("category", fs.CategoryId);
        var upo = new GridFSUploadOptions
        {
            BatchSize = 1,
            Metadata = new(metadata)
        };
        var oid = await Bucket.UploadFromStreamAsync(fs.File.FileName, fs.File.OpenReadStream(), upo);
        _ = Coll.InsertOneAsync(new()
        {
            FileId = oid.ToString(),
            FileName = fs.File.FileName,
            Length = fs.File.Length,
            ContentType = fs.File.ContentType,
            UserId = fs.UserId,
            UserName = fs.UserName,
            App = fs.App,
            BusinessType = fs.BusinessType,
            CategoryId = fs.CategoryId,
            CreatTime = DateTime.Now
        });
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
        var stream = await Bucket.OpenDownloadStreamAsync(ObjectId.Parse(id), new() { Seekable = true });
        return File(stream, stream.FileInfo.Metadata["contentType"].AsString, stream.FileInfo.Filename);
    }

    /// <summary>
    /// 通过文件名下载
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("DownloadByName/{name}")]
    public async Task<FileStreamResult> DownloadByName(string name)
    {
        var stream = await Bucket.OpenDownloadStreamByNameAsync(name, new() { Seekable = true });
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
        var fi = await (await Bucket.FindAsync("{_id:ObjectId('" + id + "')}")).SingleOrDefaultAsync() ?? throw new("no data find");
        var bytes = await Bucket.DownloadAsBytesAsync(ObjectId.Parse(id), new GridFSDownloadOptions() { Seekable = true });
        return File(bytes, fi.Metadata["contentType"].AsString, fi.Filename);
    }


    /// <summary>
    /// 通过文件名打开文件
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("FileContentByname/{name}")]
    public async Task<FileContentResult> FileContentByName(string name)
    {
        var f = Builders<GridFSFileInfo>.Filter;
        var fi = await (await Bucket.FindAsync(f.Eq(c => c.Filename, name))).FirstOrDefaultAsync() ?? throw new("can't find this file");
        var bytes = await Bucket.DownloadAsBytesByNameAsync(name, new() { Seekable = true });
        return File(bytes, fi.Metadata["contentType"].AsString, fi.Filename);
    }

    /// <summary>
    /// 打开文件内容
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <returns></returns>
    [HttpGet("FileUri/{id}")]
    public async Task<object> FileUri(string id)
    {
        if (string.IsNullOrWhiteSpace(FileSetting.PhysicalPath)) throw new("RealPath is null");
        var fi = await (await Bucket.FindAsync("{_id:ObjectId('" + id + "')}")).SingleOrDefaultAsync() ?? throw new("no data find");
        await using var mongoStream = await Bucket.OpenDownloadStreamAsync(ObjectId.Parse(id), new() { Seekable = true });
        await using var fsWrite = new FileStream($"{FileSetting.PhysicalPath}{Path.DirectorySeparatorChar}{fi.Filename}", FileMode.Create);
        var buffer = new byte[1024 * 1024];
        while (true)
        {
            var readCount = mongoStream.Read(buffer, 0, buffer.Length);
            fsWrite.Write(buffer, 0, readCount);
            if (readCount < buffer.Length) break;
        }
        return new
        {
            Uri = $"{HttpContext.Request.Scheme}:{HttpContext.Request.Host}{FileSetting.VirtualPath}/{fi.Filename}"
        };
    }

    /// <summary>
    /// 清理文件夹
    /// </summary>
    /// <returns></returns>
    [HttpDelete("ClearTempDir")]
    public Task ClearDir()
    {
        if (!Directory.Exists(FileSetting.PhysicalPath)) return Task.CompletedTask;
        try
        {
            Directory.Delete(FileSetting.PhysicalPath, true);
        }
        catch (IOException e)
        {
            Console.WriteLine(e.Message);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// 重命名文件
    /// </summary>
    /// <param name="id">文件ID</param>
    /// <param name="newname">新名称</param>
    /// <returns></returns>
    [HttpPut("{id}/Rename/{newname}")]
    public Task Rename(string id, string newname)
    {
        var filename = Coll.Find(c => c.FileId == id).Project(c => c.FileName).SingleOrDefaultAsync().Result;
        var path = $"{FileSetting.PhysicalPath}/{filename}";
        if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
        _ = Bucket.RenameAsync(ObjectId.Parse(id), newname);
        _ = Coll.UpdateOneAsync(c => c.FileId == id, Builders<GridFSItemInfo>.Update.Set(c => c.FileName, newname));
        return Task.CompletedTask;
    }

    /// <summary>
    /// 删除文件
    /// </summary>
    /// <param name="ids">文件ID集合</param>
    /// <returns></returns>
    [HttpDelete]
    public async Task Delete(params string[] ids)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < ids.Length; i++)
        {
            if (i != 0) _ = sb.Append(',');
            _ = sb.Append($"ObjectId(\"{ids[i]}\")");
        }
        var fi = await (await Bucket.FindAsync("{_id:{$in:[" + sb + "]}}")).ToListAsync();
        var fids = fi.Select(c => new
        {
            Id = c.Id.ToString(),
            FileName = c.Filename
        }).ToArray();
        Task DeleteSingleFile()
        {
            foreach (var item in fids)
            {
                _ = Bucket.DeleteAsync(ObjectId.Parse(item.Id));
                var path = $"{FileSetting.PhysicalPath}/{item.FileName}";
                if (System.IO.File.Exists(path)) System.IO.File.Delete(path);
            }
            return Task.CompletedTask;
        }
        _ = fids.Length > 6 ? Task.Run(DeleteSingleFile) : DeleteSingleFile();
        _ = Coll.DeleteManyAsync(c => ids.Contains(c.FileId));
    }
}