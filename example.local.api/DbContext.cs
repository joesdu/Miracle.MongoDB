using Miracle.MongoDB;
using MongoDB.Driver;

namespace example.local.api
{
    public class DbContext: BaseDbContext
    {
        public IMongoCollection<Test> Test => _database.GetCollection<Test>("test");
    }

    public class Test
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Sex { get; set; }
    }
}
