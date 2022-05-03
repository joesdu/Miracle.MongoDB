using MongoDB.Driver;

namespace Miracle.MongoDB;
public class MiracleMongoClientSettings
{
    public string AuthDatabase { get; set; } = "admin";
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<MongoServerAddress> ServerAddresses { get; set; } = new();
    public string DatabaseName { get; set; } = string.Empty;

    public MongoClientSettings ClientSettings
    {
        get => new()
        {
            Credential = MongoCredential.CreateCredential(AuthDatabase, UserName, Password),
            Servers = ServerAddresses
        };
    }

    public bool Validate => ServerAddresses.Count == 0 | string.IsNullOrEmpty(DatabaseName);
}
