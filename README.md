#### Miracle.MongoDB

[![LICENSE](https://img.shields.io/github/license/joesdu/Miracle.MongoDB)](https://img.shields.io/github/license/joesdu/Miracle.MongoDB) 
---
- **一旦使用本开源项目以及引用了本项目或包含本项目代码的公司因为违反劳动法(包括但不限定非法裁员,超时用工,雇佣童工等)在任何法律诉讼中败诉的,项目作者有权利追讨本项目的使用费,或者直接不允许使用任何包含本项目的源代码!任何性质的`外包公司`或`996公司`需要使用本类库,请联系作者进行商业授权!其他企业或个人可随意使用不受限.**
---
* 一个MongoDB驱动的服务包,方便使用MongoDB数据库.
* 数据库中字段名驼峰命名,ID,Id自动转化成ObjectId.
* 可配置部分类的Id字段不存为ObjectId,而存为string类型.
* 自动转化数据类型到Mongodb数据类型
```csharp
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
```
---
##### 如何使用?

* 在系统环境变量或者Docker容器中设置环境变量名称为: CONNECTIONSTRINGS_MONGO = mongodb链接字符串 或者在appsetting.json中添加
```json
{
    "ConnectionStrings": 
    {
        "Mongo": "mongodb链接字符串"
    },
    // 或者使用
    "CONNECTIONSTRINGS_MONGO": "mongodb链接字符串"
}
```

##### 使用 Miracle.MongoDB ?
* 使用Nuget安装 Miracle.MongoDB
* .Net 6 +
```csharp
using example.api;
using Miracle.MongoDB;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 添加Mongodb数据库服务
var db = builder.Services.AddMongoDbContext<DbContext>(builder.Configuration, showconnectionstring: true);
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
app.UseSwagger().UseSwaggerUI();

app.Run();
```
---
##### 使用 Miracle.MongoDB.GridFS
* 使用 Nuget 安装 Miracle.MongoDB.GridFS 和 Miracle.MongoDB
* .Net 6 +
```csharp
using example.api;
using Miracle.MongoDB;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 添加Mongodb数据库服务
var db = builder.Services.AddMongoDbContext<DbContext>(builder.Configuration, showconnectionstring: true);
// builder.Services.AddMongoDbContext<DbContext>(Configuration, c => c.AddConvertObjectIdToStringTypes(typeof(Test)));
// Miracle.MongoDB.GridFS 服务添加
builder.Services.AddMiracleGridFS(db._database, businessApp: "MiracleFS");
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "example.api", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseGlobalException();
app.UseResponseTime();

app.UseAuthorization();

app.MapControllers();
app.UseSwagger().UseSwaggerUI();

app.Run();
```
* 详细内容参见example.api项目

##### 感谢
<!-- Begin exclude from NuGet readme. -->
---

<div>
    <a href="https://www.jetbrains.com/?from=Miracle.MongoDB" align="right"><img src="https://raw.githubusercontent.com/joesdu/Miracle.MongoDB/main/documentation/jetbrains.svg" alt="JetBrains" class="logo-footer" width="72" align="left">
    <a><br/>
        
Special thanks to [JetBrains](https://www.jetbrains.com/?from=Miracle.MongoDB) for supporting us with open-source licenses for their IDEs. </a>
</div>

<!-- End exclude from NuGet readme. -->
