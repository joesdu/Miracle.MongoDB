using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace Miracle.MongoDB.GridFS.Extension;
public static class GridFSVirtualPathExtension
{
    public static IApplicationBuilder UseMiracleGridFSVirtualPath(this IApplicationBuilder app, IConfiguration config)
    {
        var miraclefile = config.GetSection(MiracleStaticFileSettings.Postion).Get<MiracleStaticFileSettings>();
        if (!Directory.Exists(miraclefile.PhysicalPath))
        {
            _ = Directory.CreateDirectory(miraclefile.PhysicalPath);
        }
        _ = app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(miraclefile.PhysicalPath),
            RequestPath = miraclefile.VirtualPath
        });
        return app;
    }
}
