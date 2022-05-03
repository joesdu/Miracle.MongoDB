using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace Miracle.MongoDB;
public class MiracleMongoOptions
{
    /// <summary>
    /// ConventionPackOptions Action
    /// </summary>
    public Action<ConventionPackOptions>? ConventionPackOptionsAction { get; set; } = null;
    /// <summary>
    /// RegistryConventionPack first
    /// </summary>
    public bool? First { get; set; } = true;
    public Dictionary<string, ConventionRegistryConfig> ConventionRegistry { get; private set; } = new()
    {
        {
            "commonpack",
            new()
            {
                Conventions = new()
                {
                    new CamelCaseElementNameConvention(), //property to camel
                    new IgnoreExtraElementsConvention(true),//
                    new NamedIdMemberConvention("Id","ID"), //_id mapping Id or ID
                    new EnumRepresentationConvention(BsonType.String) //save enum value as string
                },
                Filter = _ => true
            }
        }
    };

    public void AppendConventionRegistry(string name, ConventionRegistryConfig config) => ConventionRegistry.Add(name, config);
}

public class ConventionRegistryConfig
{
    public ConventionPack Conventions { get; set; } = new();
    public Func<Type, bool>? Filter { get; set; } = null;
}