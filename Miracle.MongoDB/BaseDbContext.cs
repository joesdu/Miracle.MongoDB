using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Miracle.MongoDB;
/// <summary>
/// mongodb base context
/// </summary>
public class BaseDbContext : IDbSet
{
    public IMongoClient? _client;
    public IMongoDatabase? _database;

    private static readonly ConventionPackOptions options = new();

    public static T CreateInstance<T>(string connectionString, string db = "") where T : BaseDbContext
    {
        var t = Activator.CreateInstance<T>();
        if (string.IsNullOrWhiteSpace(connectionString)) throw new("ConnectionString is Empty");
        var mongoUrl = new MongoUrl(connectionString);
        t._client = new MongoClient(mongoUrl);
        var dbname = string.IsNullOrWhiteSpace(db) ? mongoUrl.DatabaseName : db;
        t._database = t._client.GetDatabase(dbname);        
        return t;
    }

    public static T CreateInstance<T>(MiracleMongoClientSettings clientSettings) where T : BaseDbContext
    {
        var t = Activator.CreateInstance<T>();
        if (clientSettings.Validate) throw new("ServerAddresses or databasename is Empty");
        t._client = new MongoClient(clientSettings.ClientSettings);
        var dbname = !string.IsNullOrWhiteSpace(clientSettings.DatabaseName) ? clientSettings.DatabaseName : "miracle";
        t._database = t._client.GetDatabase(dbname);
        return t;
    }

    public static void RegistryConventionPack(MiracleMongoOptions mracleOptions)
    {
        mracleOptions.ConventionPackOptionsAction?.Invoke(options);
        if (mracleOptions.First is not null & mracleOptions.First is true)
        {
            try
            {
                foreach (var item in mracleOptions.ConventionRegistry)
                {
                    ConventionRegistry.Register(item.Key, item.Value.Conventions, item.Value.Filter);
                }
                BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Local));//to local time
                BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));//decimal to decimal default
                BsonSerializer.RegisterSerializer(new DateOnlySerializer());
                BsonSerializer.RegisterSerializer(new TimeOnlySerializer());
            }
            catch (Exception ex)
            {
                throw new($"you have already regist commonpack,please change param [first] to false from since second RegistConventionPack Method(or Miracle.MongoDB.Gen.AddMongoDbContext etc..):{ex.Message}");
            }
        }
        var idpack = new ConventionPack
        {
            new StringObjectIdIdGeneratorConvention()//Id[string] mapping ObjectId
        };
        ConventionRegistry.Register($"idpack{Guid.NewGuid()}", idpack, x => options.IsConvertObjectIdToStringType(x) == false);
    }

    protected virtual string[] GetTransactColletions() => Array.Empty<string>();

    public async Task BuildTransactCollections()
    {
        if (_database is null) throw new("_database not prepared,please use this method after DbContext instantiation");
        var transcolls = GetTransactColletions();
        if (transcolls.Length <= 0) return;
        var count = 1;
        while (await CreateCollections(transcolls) == false && count < 10)
        {
            Console.WriteLine($"[🤪]BuildTransactCollections:{count} times error,will retry at next second.[{DateTime.Now.ToLongTimeString()}]");
            count++;
            Thread.Sleep(1000);
        }
    }

    private async Task<bool> CreateCollections(IEnumerable<string> collections)
    {
        if (_database is null) throw new("_database not prepared,please use this method after DbContext instantiation");
        try
        {
            var exists = (await _database?.ListCollectionNamesAsync()!).ToList();
            var unexists = collections.Where(x => exists?.Exists(c => c == x) == false);
            foreach (var collection in unexists) _ = _database?.CreateCollectionAsync(collection)!;
            Console.WriteLine("[🎉]CreateCollections:create collections success");
            return true;
        }
        catch
        {
            return false;
        }
    }
}