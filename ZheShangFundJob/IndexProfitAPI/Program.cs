using IndexProfitAPI.Cache;
using IndexProfitAPI.IndexProfitBLL;
using log4net.Config;
using log4net.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

//log4net日志插件使用
var repository = LoggerManager.GetRepository(Assembly.GetEntryAssembly());
XmlConfigurator.Configure(repository,new FileInfo("Config/log4net.config"));
builder.Logging.AddLog4Net("Config/log4net.config");

//跨域
builder.Services.AddCors(policy =>
{
    policy.AddPolicy("Crospolicy", opt =>
    {
        opt.AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod();
    });
});
// 注册资源缓存
builder.Services.AddScoped<IAsyncResourceFilter, CacheFilter>();


builder.Services.AddControllers();
builder.Services.AddScoped<IndexProfitCalBLL>();

builder.AddServiceDefaults();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MapDefaultEndpoints();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();
app.MapControllers();
app.UseCors("Crospolicy");
app.Run();
