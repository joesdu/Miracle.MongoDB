using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Miracle.MongoDB.Gen;
/// <summary>
/// 1.Use BaseDbContext.RegistConventionPack()
/// 1.Create a DbContext use connectionString with [ConnectionStrings.Mongo in appsettings.json] or with [CONNECTIONSTRINGS_MONGO] setting value in environment variable
/// 3.Inject DbContext use services.AddSingleton(db);
/// </summary>
public static class MongoServiceCollectionExtensions
{
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
        var tipHead = "Miracle.MongoDB.Gen.AddMongoDbContext";
        var connectionString = configuration["CONNECTIONSTRINGS_MONGO"];
        if (!string.IsNullOrWhiteSpace(connectionString)) Console.WriteLine($"🎉[{tipHead}]:get [CONNECTIONSTRINGS_MONGO] setting from env succeed");
        else
        {
            connectionString = configuration.GetConnectionString("Mongo");
            Console.WriteLine($"🎉[{tipHead}]:get ConnectionStrings.Mongo from appsettings.json succeed");
        }
        if (string.IsNullOrWhiteSpace(connectionString)) throw new($"💔[{tipHead}]:no [CONNECTIONSTRINGS_MONGO] setting in env and ConnectionStrings.Mongo in appsettings.json");
        if (showconnectionstring)
        {
            Console.WriteLine($"🔗[{tipHead}]:ConnectionStrings is {connectionString}");
            Console.WriteLine($"🎗[{tipHead}]:Recommendation: The development environment is turned on and closed in the formal environment");
        }
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
        var tipHead = "Miracle.MongoDB.Gen.AddMongoDbContext";
        var connectionString = configuration[connKey];
        if (!string.IsNullOrWhiteSpace(connectionString)) Console.WriteLine($"🎉[{tipHead}]:get [{connKey}] setting from env succeed");
        else
        {
            connectionString = configuration.GetConnectionString(connKey);
            Console.WriteLine($"🎉[{tipHead}]:get {connKey} from appsettings.json succeed");
        }
        if (string.IsNullOrWhiteSpace(connectionString)) throw new($"💔[{tipHead}]:no [{connKey}] setting in env and ConnectionStrings.{connKey} in appsettings.json");
        if (showconnectionstring)
        {
            Console.WriteLine($"🔗[{tipHead}]:ConnectionStrings is {connectionString}");
            Console.WriteLine($"🎗[{tipHead}]:Recommendation: The development environment is turned on and closed in the formal environment");
        }
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
        var tipHead = "Miracle.MongoDB.Gen.AddDbSet";
        var connectionString = configuration["CONNECTIONSTRINGS_MONGO"];
        if (!string.IsNullOrWhiteSpace(connectionString)) Console.WriteLine($"🎉[{tipHead}]:get [CONNECTIONSTRINGS_MONGO] setting from env succeed");
        else
        {
            connectionString = configuration.GetConnectionString("Mongo");
            Console.WriteLine($"🎉[{tipHead}]:get ConnectionStrings.Mongo in appsettings.json succeed");
        }
        if (string.IsNullOrWhiteSpace(connectionString)) throw new($"💔[{tipHead}]:no [CONNECTIONSTRINGS_MONGO] setting in env and ConnectionStrings.Mongo in appsettings.json");
        if (showconnectionstring)
        {
            Console.WriteLine($"🔗[{tipHead}]:ConnectionStrings is {connectionString}");
            Console.WriteLine($"🎗[{tipHead}]:Recommendation: The development environment is turned on and closed in the formal environment");
        }
        BaseDbContext.RegistConventionPack(conventionPackOptionsAction, first);
        var db = BaseDbContext.CreateInstance<T>(connectionString);
        db.BuildTransactCollections();
        _ = services.AddSingleton(typeof(IDbSet), db);
        return db;
    }
}