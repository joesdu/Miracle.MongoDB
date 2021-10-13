using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Miracle.MongoDB
{
    /// <summary>
    /// mongodb base context
    /// </summary>
    public class BaseDbContext : IDbSet
    {
        public IMongoClient _client;
        public IMongoDatabase _database;
        public IMongoCollection<BsonDocument> GetCollection(string collection) => _database.GetCollection<BsonDocument>(collection);
        private static readonly ConventionPackOptions options = new();

        public static T CreateInstance<T>(CoreAppSetting dbSettings) where T : BaseDbContext
        {
            T t = Activator.CreateInstance<T>();
            if (dbSettings.Servers.Count == 0) throw new("BaseDbContext Init error! host,port,db must not null");
            MongoCredential credential = null;
            if (dbSettings.Credential.User != null || dbSettings.Credential.Pwd != null) credential = MongoCredential.CreateCredential("admin", dbSettings.Credential.User, dbSettings.Credential.Pwd);
            var settings = new MongoClientSettings
            {
                Credential = credential,
                ReplicaSetName = dbSettings.ReplSetName
            };
            if (dbSettings.Servers.Count > 1 && !string.IsNullOrWhiteSpace(dbSettings.ReplSetName)) settings.Servers = dbSettings.Servers.Select(x => new MongoServerAddress(x.Host, x.Port));
            else settings.Server = new(dbSettings.Servers[0].Host, dbSettings.Servers[0].Port);
            if (dbSettings.ServerSelectionTimeout is not null and not 0)
            {
                settings.ServerSelectionTimeout = new(0, 0, 0, 0, dbSettings.ServerSelectionTimeout.Value);
            }
            t._client = new MongoClient(settings);
            t._database = t._client.GetDatabase(dbSettings.Db);
            return t;
        }

        public static T CreateInstance<T>(string connectionString, string db = "") where T : BaseDbContext
        {
            T t = Activator.CreateInstance<T>();
            if (string.IsNullOrWhiteSpace(connectionString)) throw new("connectionString is empty");
            var mongoUrl = new MongoUrl(connectionString);
            t._client = new MongoClient(mongoUrl);
            var dbname = string.IsNullOrWhiteSpace(db) ? mongoUrl.DatabaseName : db;
            t._database = t._client.GetDatabase(dbname);
            return t;
        }

        public static void RegistConventionPack(Action<ConventionPackOptions> configure = null, bool first = true)
        {
            configure?.Invoke(options);
            if (first)
            {
                try
                {
                    var pack = new ConventionPack
                    {
                        new CamelCaseElementNameConvention(),//property to camel
                        new IgnoreExtraElementsConvention(true),//
                        new NamedIdMemberConvention("Id","ID"),//_id mapping Id or ID
                        new EnumRepresentationConvention(BsonType.String),//save enum value as string
                    };
                    ConventionRegistry.Register("commonpack", pack, x => true);
                    BsonSerializer.RegisterSerializer(typeof(DateTime), new DateTimeSerializer(DateTimeKind.Local));//to local time
                    BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));//decimal to decimal default
                }
                catch (Exception ex)
                {
                    throw new("you have already regist commonpack,please change param [first] to false from since second RegistConventionPack Method(or BL.MongoDB.Gen.AddBLDbContext etc..):" + ex.Message);
                }
            }
            var idpack = new ConventionPack
            {
                new StringObjectIdIdGeneratorConvention()//Id[string] mapping ObjectId
            };
            ConventionRegistry.Register("idpack" + Guid.NewGuid().ToString(), idpack, x => options.IsConvertObjectIdToStringType(x) == false);
        }

        protected virtual string[] GetTransactColletions() => Array.Empty<string>();

        public void BuildTransactCollections()
        {
            if (_database is null) throw new("_database not prepared,please use this method after DbContext instantiation");
            var transcolls = GetTransactColletions();
            if (transcolls.Length > 0)
            {
                int count = 1;
                while (CreateCollections(transcolls) == false && count < 10)
                {
                    Console.WriteLine($"[🤪]BuildTransactCollections:{count} times error,will retry at next second.[{DateTime.Now.ToLongTimeString()}]");
                    count++;
                    Thread.Sleep(1000);
                }
            }
        }

        private bool CreateCollections(string[] collections)
        {
            try
            {
                var exists = _database.ListCollectionNames().ToList();
                var unexists = collections.Where(x => exists.Exists(c => c == x) == false);
                foreach (var collection in unexists) _database.CreateCollection(collection);
                Console.WriteLine($"[🎉]CreateCollections:create collections success");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
