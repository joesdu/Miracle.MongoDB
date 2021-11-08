using Miracle.MongoDB;
using Miracle.MongoDB.Gen;
using Miracle.MongoDB.GridFS;
using Miracle.WebApi.Filters;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "example.local.api", Version = "v1" }));
builder.Services.AddCors(c => c.AddPolicy("AllowedHosts", c => c.WithOrigins(builder.Configuration["AllowedHosts"].Split(",")).AllowAnyMethod().AllowAnyHeader()));

var db = builder.Services.AddMongoDbContext<BaseDbContext>(builder.Configuration, showconnectionstring: true);

builder.Services.AddMiracleGridFS(db._database, businessApp: "MiracleFS");

builder.Services.AddControllers(c => c.Filters.Add<ActionExecuteFilter>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();
app.UseGlobalException();
app.UseResponseTime();
app.UseCors("AllowedHosts");

app.UseAuthorization();

app.MapControllers();
app.UseSwagger().UseSwaggerUI();

app.Run();