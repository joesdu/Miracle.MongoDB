# Miracle.MongoDB

* 一个MongoDB驱动的服务包,方便使用MongoDB数据库.
* 数据库中字段名驼峰命名,ID,Id自动转化成ObjectId.
* 可配置部分类的Id字段不存为ObjectId,而存为string类型.
* 自动转化数据类型到Mongodb数据类型
```csharp
value.BsonType switch
{
    BsonType.Array => value.AsBsonArray.ToArray().Select(x => x.GetValue()),
    BsonType.Boolean => value.AsBoolean,
    BsonType.DateTime => value.ToUniversalTime(),
    BsonType.Decimal128 => value.AsDecimal,
    BsonType.Document => totype is null ? value.ToJson() : BsonSerializer.Deserialize(value.ToBsonDocument(), totype),
    BsonType.Double => value.AsDouble,
    BsonType.Int32 => value.AsInt32,
    BsonType.Int64 => value.AsInt64,
    BsonType.Null => null,
    BsonType.ObjectId => value.AsString,
    BsonType.String => value.AsString,
    BsonType.Timestamp => value.AsString,
    _ => null
};
```
---
# 如何使用?

* 在系统环境变量或者Docker容器中设置环境变量名称为: CONNECTIONSTRINGS_MONGO = mongodb链接字符串 或者在appsetting.json中添加
```json
{
    "ConnectionStrings": 
    {
        "Mongo": "mongodb链接字符串"
    }
    // 或者使用
    "CONNECTIONSTRINGS_MONGO": "mongodb链接字符串"
}
```

## 使用 Miracle.MongoDB.Gen ?
* 使用Nuget安装 Miracle.MongoDB.Gen
* .Net 6 +
```csharp
using example.api;
using Miracle.MongoDB.Gen;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 添加Mongodb数据库服务
var db = builder.Services.AddMongoDbContext<DbContext>(builder.Configuration);
// builder.Services.AddMongoDbContext<DbContext>(Configuration, c => c.AddConvertObjectIdToStringTypes(typeof(Test)));
//

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "example.api", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseGlobalException();
app.UseResponseTime();

app.UseAuthorization();

app.MapControllers();
app.UseSwagger().UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "example.api v1"));

app.Run();
```
---
## 使用 Miracle.MongoDB.GridFS
* 使用 Nuget 安装 Miracle.MongoDB.GridFS 和 Miracle.MongoDB.Gen
* .Net 6 +
```csharp
using example.api;
using Miracle.MongoDB.Gen;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 添加Mongodb数据库服务
var db = builder.Services.AddMongoDbContext<DbContext>(builder.Configuration);
// builder.Services.AddMongoDbContext<DbContext>(Configuration, c => c.AddConvertObjectIdToStringTypes(typeof(Test)));
// Miracle.MongoDB.GridFS 服务添加
builder.Services.AddMiracleGridFS(db._database, new() { });
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
```
* 详细内容参见example.api项目
