using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace example.local.api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    private readonly DbContext db;
    private readonly FilterDefinitionBuilder<Test> bf = Builders<Test>.Filter;
    public TestController(DbContext dbContext) => db = dbContext;

    [HttpPost]
    public Task Post()
    {
        var o = new Test
        {
            DateTime = DateTime.Now,
            TimeSpan = TimeSpan.FromMilliseconds(50000d),
            DateOnly = DateOnly.FromDateTime(DateTime.Now),
            TimeOnly = TimeOnly.FromDateTime(DateTime.Now)
        };
        _ = db.Test.InsertOneAsync(o);
        return Task.CompletedTask;
    }
    [HttpGet]
    public async Task<IEnumerable<Test>> Get()
    {
        return await db.Test.Find(bf.Empty).ToListAsync();
    }
}
