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
    /// <summary>
    /// Show Connection String,Recommendation: The development environment is turned on and closed in the formal environment
    /// </summary>
    public bool? ShowConnectionString { get; set; } = false;
}