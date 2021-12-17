﻿using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Miracle.MongoDB.GridFS;
public static class GridFSExtensions
{
    public static string BusinessApp { get; set; } = string.Empty;
    public static IServiceCollection AddMiracleGridFS(this IServiceCollection services, IMongoDatabase db, MiracleGridFSOptions? options)
    {
        if (db is null) throw new("db can't be null");
        options ??= new();
        BusinessApp = options.BusinessApp;
        _ = services.Configure<FormOptions>(c =>
        {
            c.MultipartBodyLengthLimit = long.MaxValue;
            c.ValueLengthLimit = int.MaxValue;
        }).Configure<KestrelServerOptions>(c => c.Limits.MaxRequestBodySize = int.MaxValue).AddSingleton(new GridFSBucket(options.DefalutDB ? db.Client.GetDatabase("miracle") : db, options.Options));
        _ = services.AddSingleton(db.Client.GetDatabase("miracle").GetCollection<GridFSItemInfo>(options.ItemInfo));
        return services;
    }

    public static async Task<IServiceCollection> AddMiracleMongoAndGridFS<T>(this IServiceCollection services, IConfiguration configuration, MiracleMongoOptions? dboption, MiracleGridFSOptions? options) where T : BaseDbContext
    {
        var db = await services.AddMongoDbContext<T>(configuration, dboption);
        return AddMiracleGridFS(services, db._database!, options);
    }
}