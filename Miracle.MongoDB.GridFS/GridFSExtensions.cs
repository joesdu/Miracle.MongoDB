using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Miracle.MongoDB.GridFS
{
    public static class GridFSExtensions
    {
        public static IServiceCollection AddMiracleGridFS(this IServiceCollection services, IMongoDatabase db, GridFSBucketOptions options = null, string businessApp = null, bool defalutdb = true)
        {
            if (db is null) throw new("db can't be null");
            BusinessApp = businessApp;
            _ = services.Configure<FormOptions>(c =>
            {
                c.MultipartBodyLengthLimit = long.MaxValue;
                c.ValueLengthLimit = int.MaxValue;
            }).Configure<KestrelServerOptions>(c => c.Limits.MaxRequestBodySize = int.MaxValue).AddSingleton(new GridFSBucket(defalutdb ? db.Client.GetDatabase("mircale") : db, options));
            return services;
        }
        public static string BusinessApp { get; set; }
    }
}
