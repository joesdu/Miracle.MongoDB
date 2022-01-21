#### Miracle.MongoDB

## [![LICENSE](https://img.shields.io/github/license/joesdu/Miracle.MongoDB)](https://img.shields.io/github/license/joesdu/Miracle.MongoDB)

- **一旦使用本开源项目以及引用了本项目或包含本项目代码的公司因为违反劳动法(包括但不限定非法裁员,超时用工,雇佣童工等)在任何法律诉讼中败诉的,项目作者有权利追讨本项目的使用费,或者直接不允许使用任何包含本项目的源代码!任何性质的`外包公司`或`996公司`需要使用本类库,请联系作者进行商业授权!其他企业或个人可随意使用不受限.**

---

- 一个 MongoDB 驱动的服务包,方便使用 MongoDB 数据库.
- 数据库中字段名驼峰命名,ID,Id 自动转化成 ObjectId.
- 可配置部分类的 Id 字段不存为 ObjectId,而存为 string 类型.
- 自动转化数据类型到 MongoDB 数据类型
- 自动本地化 MOngoDB 时间类型

---

##### 如何使用?

- 在系统环境变量或者 Docker 容器中设置环境变量名称为: CONNECTIONSTRINGS_MONGO = mongodb 链接字符串 或者在 appsetting.json 中添加

```json
{
  "ConnectionStrings": {
    "Mongo": "mongodb链接字符串"
  },
  // 或者使用
  "CONNECTIONSTRINGS_MONGO": "mongodb链接字符串",
  // 添加文件缓存
  "MiracleStaticFile": {
    "VirtualPath": "/miraclefs",
    "PhysicalPath": "D:\\test"
  }
}
```

##### 使用 Miracle.MongoDB ?

- 使用 Nuget 安装 Miracle.MongoDB
- .Net 6 +

```csharp
using example.api;
using Miracle.MongoDB;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 添加Mongodb数据库服务
builder.Services.AddMongoDbContext<BaseDbContext>(builder.Configuration, new ()
{
    ShowConnectionString = true,
    // ConventionPackOptionsAction = c => c.AddConvertObjectIdToStringTypes(typeof(Test)),
    // First = true
});

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

---

##### 使用 Miracle.MongoDB.GridFS

###### ChangeLogs
- 1.3.6
- 新增文件缓存到物理路径,便于文件在线使用.
- 添加物理路径清理接口.(可通过调用该接口定时清理所有缓存的文件)

---

- 使用 Nuget 安装 Miracle.MongoDB.GridFS 和 Miracle.MongoDB
- .Net 6 +

```csharp
using example.api;
using Miracle.MongoDB;
using Miracle.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 添加Mongodb数据库服务
//var db = await builder.Services.AddMongoDbContext<BaseDbContext>(builder.Configuration, new()
//{
//    ShowConnectionString = true,
//    ConventionPackOptionsAction = c => c.AddConvertObjectIdToStringTypes(typeof(Test)),
//    First = true
//});

// Miracle.MongoDB.GridFS 服务添加
//builder.Services.AddMiracleGridFS(db._database!, new()
//{
//    BusinessApp = "MiracleFS",
//    Options = new ()
//    {
//        BucketName = "",
//        ChunkSizeBytes = 1024,
//        DisableMD5 = true,
//        ReadConcern = new () {},
//        ReadPreference = ReadPreference.Primary,
//        WriteConcern = WriteConcern.Unacknowledged
//    }
//    DefalutDB = true,
//    ItemInfo = "item.info"
//});

// 若是使用GridFS库,可以直接同时添加MongoDB服务和GridFS服务
await builder.Services.AddMiracleMongoAndGridFS<BaseDbContext>(builder.Configuration, new()
{
    ShowConnectionString = true
}, new()
{
    BusinessApp = "MiracleFS"
});

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "example.api", Version = "v1" }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

app.UseGlobalException();
app.UseResponseTime();

app.UseAuthorization();

// 或者使用MiracleStaticFileSettings类型绑定
// var staticfile = builder.Configuration.GetSection(MiracleStaticFileSettings.Postion).Get<MiracleStaticFileSettings>();
// 获取物理路径是否存在,不存在就创建一个
if (!Directory.Exists(builder.Configuration["MiracleStaticFile:PhysicalPath"]))
{
    Directory.CreateDirectory(builder.Configuration["MiracleStaticFile:PhysicalPath"]);
}
// 添加静态文件服务
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(builder.Configuration["MiracleStaticFile:PhysicalPath"]),
    RequestPath = builder.Configuration["MiracleStaticFile:VirtualPath"]
});
app.MapControllers();
app.UseSwagger().UseSwaggerUI();

app.Run();
```

- 详细内容参见 example.api 项目

##### 感谢

## <!-- Begin exclude from NuGet readme. -->

<div>
    <a href="https://www.jetbrains.com/?from=Miracle.MongoDB" align="right"><img src="https://raw.githubusercontent.com/joesdu/Miracle.MongoDB/main/jetbrains.svg" alt="JetBrains" class="logo-footer" width="72" align="left">
    <a><br/>
        
Special thanks to [JetBrains](https://www.jetbrains.com/?from=Miracle.MongoDB) for supporting us with open-source licenses for their IDEs. </a>
</div>

<!-- End exclude from NuGet readme. -->
