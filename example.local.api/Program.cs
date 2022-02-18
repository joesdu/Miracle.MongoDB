using example.local.api;
using Microsoft.Extensions.FileProviders;
using Miracle.MongoDB;
using Miracle.MongoDB.GridFS;
using Miracle.WebCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "example.local.api", Version = "v1" }));
builder.Services.AddCors(c => c.AddPolicy("AllowedHosts", s => s.WithOrigins(builder.Configuration["AllowedHosts"].Split(",")).AllowAnyMethod().AllowAnyHeader()));
var db = await builder.Services.AddMongoDbContext<DbContext>(builder.Configuration, new()
{
    ShowConnectionString = true
});

//builder.Services.AddMiracleGridFS(db._database!, new()
//{
//    BusinessApp = "MiracleFS"
//});
//await builder.Services.AddMiracleMongoAndGridFS<BaseDbContext>(builder.Configuration, new()
//{
//    ShowConnectionString = true
//}, fsoptions: new()
//{
//    BusinessApp = "MiracleFS",
//    //Options = new ()
//    //{
//    //    BucketName = "",
//    //    ChunkSizeBytes = 1024,
//    //    DisableMD5 = true,
//    //    ReadConcern = new () {},
//    //    ReadPreference = ReadPreference.Primary,
//    //    WriteConcern = WriteConcern.Unacknowledged
//    //}
//    DefalutDB = true,
//    ItemInfo = "item.info"
//});
builder.Services.AddControllers(c => c.Filters.Add<ActionExecuteFilter>()).AddJsonOptions(c =>
{
    c.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.DateTimeConverter());
    c.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    c.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.TimeOnlyJsonConverter());
    c.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.DateOnlyJsonConverter());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) _ = app.UseDeveloperExceptionPage();
app.UseGlobalException();
app.UseResponseTime();
app.UseCors("AllowedHosts");

app.UseAuthorization();

//var miraclefile = builder.Configuration.GetSection(MiracleStaticFileSettings.Postion).Get<MiracleStaticFileSettings>();

//if (!Directory.Exists(miraclefile.PhysicalPath))
//{
//    _ = Directory.CreateDirectory(miraclefile.PhysicalPath);
//}
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(miraclefile.PhysicalPath),
//    RequestPath = miraclefile.VirtualPath
//});
app.MapControllers();
app.UseSwagger().UseSwaggerUI();

app.Run();