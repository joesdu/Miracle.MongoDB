namespace Miracle.MongoDB
{
    public class CoreAppSetting
    {
        public string ReplSetName { get; set; }
        public string Db { get; set; }
        public int? ServerSelectionTimeout { get; set; }
        public List<ServerItem> Servers { get; set; } = new();
        public Credential Credential { get; set; }
    }
    public class ServerItem
    {
        public string Host { get; set; }
        public int Port { get; set; }

    }
    public class Credential
    {
        public string User { get; set; }
        public string Pwd { get; set; }
    }
}
