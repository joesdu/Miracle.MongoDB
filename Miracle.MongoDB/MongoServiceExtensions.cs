using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Miracle.MongoDB;
/// <summary>
/// 1.Use BaseDbContext.RegistryConventionPack()
/// 1.Create a DbContext use connectionString with [ConnectionStrings.Mongo in appsettings.json] or with [CONNECTIONSTRINGS_MONGO] setting value in environment variable
/// 3.Inject DbContext use services.AddSingleton(db);
/// </summary>
public static class MongoServiceExtensions
{
    private enum WriteType
    {
        Info,
        Tips
    }
    private static void WriteTxt(string txt, WriteType type = WriteType.Info)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        string header;
        ConsoleColor forecolor;
        switch (type)
        {
            case WriteType.Info:
                header = "🔗[Info]: ";
                forecolor = ConsoleColor.Cyan;
                break;
            case WriteType.Tips:
                header = "💡[Tips]: ";
                forecolor = ConsoleColor.Yellow;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        Console.Write(header);
        Console.ForegroundColor = forecolor;
        Console.WriteLine(txt);
        Console.ForegroundColor = ConsoleColor.White;
    }
    /// <summary>
    /// 获取连接字符串,并提供一些信息输出
    /// </summary>
    /// <param name="configuration">.Net Configuration</param>
    /// <param name="connKey">Connection Keyword</param>
    /// <param name="showconnectionstring">Show Connection String,Recommendation: The development environment is turned on and closed in the formal environment</param>
    /// <returns></returns>
    private static string ConnectionString(IConfiguration configuration, string connKey = "CONNECTIONSTRINGS_MONGO", bool? showconnectionstring = false)
    {
        var connectionString = configuration[connKey];
        if (!string.IsNullOrWhiteSpace(connectionString)) WriteTxt("Get [CONNECTIONSTRINGS_MONGO] setting from env succeed");
        else
        {
            connectionString = configuration.GetConnectionString("Mongo");
            WriteTxt("Get ConnectionStrings.Mongo in appsettings.json succeed");
        }
        if (string.IsNullOrWhiteSpace(connectionString)) throw new("💔:No [CONNECTIONSTRINGS_MONGO] setting in env and ConnectionStrings.Mongo in appsettings.json");
        if (showconnectionstring is not null & showconnectionstring is not false) return connectionString;
        WriteTxt($"ConnectionStrings is {connectionString}");
        WriteTxt("Set showconnectionstring = false in production environments", WriteType.Tips);
        return connectionString;
    }

    /// <summary>
    /// Add DbContext Service Use Connection String
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.DbContext</typeparam>
    /// <param name="services">.Net Services</param>
    /// <param name="configuration">.Net Configuration</param>
    /// <param name="dboptions">DbContextOptions</param>
    /// <returns></returns>
    public static async Task<T> AddMongoDbContext<T>(this IServiceCollection services, IConfiguration configuration, MiracleMongoOptions? dboptions = null) where T : BaseDbContext
    {
        dboptions ??= new();
        var connectionString = ConnectionString(configuration, showconnectionstring: dboptions.ShowConnectionString);
        BaseDbContext.RegistryConventionPack(dboptions.ConventionPackOptionsAction, dboptions.First);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        await db.BuildTransactCollections();
        _ = services.AddSingleton(db);
        return db;
    }

    /// <summary>
    /// Add DbContext Service Use Custom connection string with custom key name
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.BaseDbContext</typeparam>
    /// <param name="services">.Net Services</param>
    /// <param name="configuration">.Net Configuration</param>
    /// <param name="connKey">Connection Keyword</param>
    /// <param name="dboptions">DbContextOptions</param>
    /// <returns></returns>
    public static async Task<T> AddMongoDbContextWithSpecificKey<T>(this IServiceCollection services, IConfiguration configuration, string connKey, MiracleMongoOptions? dboptions = null) where T : BaseDbContext
    {
        dboptions ??= new();
        var connectionString = ConnectionString(configuration, connKey, dboptions.ShowConnectionString);
        BaseDbContext.RegistryConventionPack(dboptions.ConventionPackOptionsAction, dboptions.First);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        await db.BuildTransactCollections();
        _ = services.AddSingleton(db);
        return db;
    }

    /// <summary>
    /// Add IDbSet Service Use Connection string
    /// </summary>
    /// <typeparam name="T">Miracle.MongoDB.IDbSet</typeparam>
    /// <param name="services">.Net Services</param>
    /// <param name="configuration">.Net Configuration</param>
    /// <param name="dboptions">DbContextOptions</param>
    /// <returns></returns>
    public static async Task<T> AddMongoDbSet<T>(this IServiceCollection services, IConfiguration configuration, MiracleMongoOptions? dboptions = null) where T : BaseDbContext, IDbSet
    {
        dboptions ??= new();
        var connectionString = ConnectionString(configuration, showconnectionstring: dboptions.ShowConnectionString);
        BaseDbContext.RegistryConventionPack(dboptions.ConventionPackOptionsAction, dboptions.First);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        await db.BuildTransactCollections();
        _ = services.AddSingleton(typeof(IDbSet), db);
        return db;
    }
}