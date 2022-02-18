using Miracle.MongoDB;
using MongoDB.Driver;

namespace example.local.api;
public class DbContext : BaseDbContext
{
    /// <summary>
    /// 作息时间管理
    /// </summary>
    public IMongoCollection<Test> Test => _database!.GetCollection<Test>("test");
    
}
public class Test
{
    public DateTime DateTime { get; set; }
    public TimeSpan TimeSpan { get; set; }
    public DateOnly DateOnly { get; set; }
    public TimeOnly TimeOnly { get; set; }
}