using example.api;
using Miracle.MongoDB.GridFS;
using Miracle.WebCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(c => c.AddPolicy("AllowedHosts", s => s.WithOrigins(builder.Configuration["AllowedHosts"].Split(",")).AllowAnyMethod().AllowAnyHeader()));

//await builder.Services.AddMongoDbContext<DbContext>(builder.Configuration, new()
//{
//    ShowConnectionString = false
//});

await builder.Services.AddMiracleMongoAndGridFS<DbContext>(builder.Configuration, new ()
{
    ShowConnectionString = false
},new()
{
    BusinessApp="MiracleFS"
});

builder.Services.AddControllers(c => c.Filters.Add<ActionExecuteFilter>());

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "example.api", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) _ = app.UseDeveloperExceptionPage();
app.UseGlobalException();
app.UseResponseTime();
app.UseCors("AllowedHosts");

app.UseAuthorization();

app.MapControllers();
app.UseSwagger().UseSwaggerUI();

app.Run();
