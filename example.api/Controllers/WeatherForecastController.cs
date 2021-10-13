using Microsoft.AspNetCore.Mvc;

namespace example.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly DbContext db;

        public WeatherForecastController(DbContext context) => db = context;

        private static readonly string[] Summaries = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

        [HttpGet]
        public async Task<object> Get()
        {
            var o = Enumerable.Range(1, 5).Select(index => new Test
            {
                Sex = Random.Shared.Next(-20, 55),
                Name = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            await db.Test.InsertManyAsync(o);
            return o;
        }
    }
}