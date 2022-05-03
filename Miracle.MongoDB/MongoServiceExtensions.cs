using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Miracle.MongoDB;
/// <summary>
/// 1.Create a DbContext use connectionString with [ConnectionStrings.Mongo in appsettings.json] or with [CONNECTIONSTRINGS_MONGO] setting value in environment variable
/// 2.Inject DbContext use services.AddSingleton(db);
/// </summary>
public static class MongoServiceExtensions
{
    /// <summary>
    /// 获取连接字符串,并提供一些信息输出
    /// </summary>
    /// <param name="configuration">IConfiguration</param>
    /// <param name="connKey">Connection Keyword</param>
    /// <param name="showconnectionstring">Show Connection String,Recommendation: The development environment is turned on and closed in the formal environment</param>
    /// <returns></returns>
    private static string ConnectionString(IConfiguration configuration, string connKey = "CONNECTIONSTRINGS_MONGO")
    {
        var connectionString = configuration[connKey];
        if (!string.IsNullOrWhiteSpace(connectionString)) throw new("ConnectionString is not null");
        else connectionString = configuration.GetConnectionString("Mongo");
        if (string.IsNullOrWhiteSpace(connectionString)) throw new("💔:No [CONNECTIONSTRINGS_MONGO] setting in env and ConnectionStrings.Mongo in appsettings.json");
        return connectionString;
    }

    /// <summary>
    /// Add DbContext Service Use Connection String
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.DbContext</typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="configuration">IConfiguration</param>
    /// <param name="dboptions">DbContextOptions</param>
    /// <returns></returns>
    public static async Task<T> AddMongoDbContext<T>(this IServiceCollection services, IConfiguration configuration, string connKey = "CONNECTIONSTRINGS_MONGO", MiracleMongoOptions? dboptions = null) where T : BaseDbContext
    {
        dboptions ??= new();
        var connectionString = ConnectionString(configuration, connKey);
        BaseDbContext.RegistryConventionPack(dboptions);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        await db.BuildTransactCollections();
        _ = services.AddSingleton(db);
        return db;
    }

    /// <summary>
    /// Add DbContext Service Use Connection String
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.DbContext</typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="configuration">IConfiguration</param>
    /// <param name="dboptions">DbContextOptions</param>
    /// <returns></returns>
    public static async Task<T> AddMongoDbContext<T>(this IServiceCollection services, MiracleMongoClientSettings clientSettings, MiracleMongoOptions? dboptions = null) where T : BaseDbContext
    {
        dboptions ??= new();
        BaseDbContext.RegistryConventionPack(dboptions);
        var db = BaseDbContext.CreateInstance<T>(clientSettings);
        await db.BuildTransactCollections();
        _ = services.AddSingleton(db);
        return db;
    }

    /// <summary>
    /// Add IDbSet Service Use Connection string
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.IDbSet</typeparam>
    /// <param name="services">IServiceCollection</param>
    /// <param name="configuration">IConfiguration</param>
    /// <param name="dboptions">DbContextOptions</param>
    /// <returns></returns>
    public static async Task<T> AddMongoDbSet<T>(this IServiceCollection services, IConfiguration configuration, MiracleMongoOptions? dboptions = null) where T : BaseDbContext, IDbSet
    {
        dboptions ??= new();
        var connectionString = ConnectionString(configuration);
        BaseDbContext.RegistryConventionPack(dboptions);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        await db.BuildTransactCollections();
        _ = services.AddSingleton(typeof(IDbSet), db);
        return db;
    }
}