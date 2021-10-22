using example.api;
using Miracle.MongoDB.Gen;
using Miracle.MongoDB.GridFS;
using Miracle.WebApi.Filters;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(c => c.AddPolicy("AllowedHosts", c => c.WithOrigins(builder.Configuration.GetValue<string>("AllowedHosts").Split(",")).AllowAnyMethod().AllowAnyHeader()));

var db = builder.Services.AddMongoDbContext<DbContext>(builder.Configuration);

builder.Services.AddMiracleGridFS(db._database);

builder.Services.AddControllers(c =>
{
    c.Filters.Add<ExceptionFilter>();
    c.Filters.Add<ActionExecuteFilter>();
});

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "example.api", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

//app.UseGlobalException();
app.UseResponseTime();
app.UseCors("AllowedHosts");

app.UseAuthorization();

app.MapControllers();
app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "example.api v1"));

app.Run();
