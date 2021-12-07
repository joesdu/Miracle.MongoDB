﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Miracle.MongoDB;
/// <summary>
/// 1.Use BaseDbContext.RegistConventionPack()
/// 1.Create a DbContext use connectionString with [ConnectionStrings.Mongo in appsettings.json] or with [CONNECTIONSTRINGS_MONGO] setting value in environment variable
/// 3.Inject DbContext use services.AddSingleton(db);
/// </summary>
public static class MongoServiceCollectionExtensions
{
    private static void WriteInfo(string info)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"🔗[Info]: ");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(info);
        Console.ForegroundColor = ConsoleColor.White;
    }
    private static void WriteTips(string tips)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"💡[Tips]: ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(tips);
        Console.ForegroundColor = ConsoleColor.White;
    }
    /// <summary>
    /// 获取连接字符串,并提供一些信息输出
    /// </summary>
    /// <param name="configuration">.Net Configuration</param>
    /// <param name="connKey">Connection Keyword</param>
    /// <param name="showconnectionstring">Show Connection String,Recommendation: The development environment is turned on and closed in the formal environment</param>
    /// <returns></returns>
    private static string ConnectionString(IConfiguration configuration, string connKey = "CONNECTIONSTRINGS_MONGO", bool showconnectionstring = false)
    {
        var connectionString = configuration[connKey];
        if (!string.IsNullOrWhiteSpace(connectionString)) WriteInfo($"Get [CONNECTIONSTRINGS_MONGO] setting from env succeed");
        else
        {
            connectionString = configuration.GetConnectionString("Mongo");
            WriteInfo("Get ConnectionStrings.Mongo in appsettings.json succeed");
        }
        if (string.IsNullOrWhiteSpace(connectionString)) throw new($"💔:No [CONNECTIONSTRINGS_MONGO] setting in env and ConnectionStrings.Mongo in appsettings.json");
        if (showconnectionstring)
        {
            WriteInfo($"ConnectionStrings is {connectionString}");
            WriteTips("Set showconnectionstring = false in production environments");
        }
        return connectionString;
    }
    /// <summary>
    /// Add DbContext Service Use Connection String
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.DbContext</typeparam>
    /// <param name="services">.Net Services</param>
    /// <param name="configuration">.Net Configuration</param>
    /// <param name="conventionPackOptionsAction">ConventionPackOptions Action</param>
    /// <param name="first">RegistConventionPack first</param>
    /// <param name="showconnectionstring">Show Connection String,Recommendation: The development environment is turned on and closed in the formal environment</param>
    /// <returns></returns>
    public static T AddMongoDbContext<T>(this IServiceCollection services, IConfiguration configuration, Action<ConventionPackOptions>? conventionPackOptionsAction = null, bool first = true, bool showconnectionstring = false) where T : BaseDbContext
    {
        var connectionString = ConnectionString(configuration, showconnectionstring: showconnectionstring);
        BaseDbContext.RegistConventionPack(conventionPackOptionsAction, first);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        db.BuildTransactCollections();
        _ = services.AddSingleton(db);
        return db;
    }
    /// <summary>
    /// Add DbContext Service Use Custom connection string with custom keyname
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.BaseDbContext</typeparam>
    /// <param name="services">.Net Services</param>
    /// <param name="configuration">.Net Configuration</param>
    /// <param name="connKey">Connection Keyword</param>
    /// <param name="conventionPackOptionsAction">ConventionPackOptions Action</param>
    /// <param name="first">RegistConventionPack first</param>
    /// <param name="showconnectionstring">Show Connection String,Recommendation: The development environment is turned on and closed in the formal environment</param>
    /// <returns></returns>
    public static T AddMongoDbContextSpecificConnKey<T>(this IServiceCollection services, IConfiguration configuration, string connKey, Action<ConventionPackOptions>? conventionPackOptionsAction = null, bool first = true, bool showconnectionstring = false) where T : BaseDbContext
    {
        var connectionString = ConnectionString(configuration, connKey, showconnectionstring);
        BaseDbContext.RegistConventionPack(conventionPackOptionsAction, first);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        db.BuildTransactCollections();
        _ = services.AddSingleton(db);
        return db;
    }
    /// <summary>
    /// Add IDbSet Service Use Connection string
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.IDbSet</typeparam>
    /// <param name="services">.Net Services</param>
    /// <param name="configuration">.Net Configuration</param>
    /// <param name="conventionPackOptionsAction">ConventionPackOptions Action</param>
    /// <param name="first">RegistConventionPack first</param>
    /// <param name="showconnectionstring">Show Connection String,Recommendation: The development environment is turned on and closed in the formal environment</param>
    /// <returns></returns>
    public static T AddMongoDbSet<T>(this IServiceCollection services, IConfiguration configuration, Action<ConventionPackOptions>? conventionPackOptionsAction = null, bool first = true, bool showconnectionstring = false) where T : BaseDbContext, IDbSet
    {
        var connectionString = ConnectionString(configuration, showconnectionstring: showconnectionstring);
        BaseDbContext.RegistConventionPack(conventionPackOptionsAction, first);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        db.BuildTransactCollections();
        _ = services.AddSingleton(typeof(IDbSet), db);
        return db;
    }
}