using example.api;
using Miracle.MongoDB.Gen;
using Miracle.MongoDB.GridFS;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var db = builder.Services.AddMongoDbContext<DbContext>(builder.Configuration);

builder.Services.AddMiracleGridFS(db._database);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "example.api", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseGlobalException();
app.UseResponseTime();

app.UseAuthorization();

app.MapControllers();
app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "example.api v1"));

app.Run();
