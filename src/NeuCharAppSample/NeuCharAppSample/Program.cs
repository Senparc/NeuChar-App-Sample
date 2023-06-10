using Microsoft.Extensions.Configuration;
using NeuCharAppSample.Configs;
using NeuCharAppSample.MessageHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//使用本地缓存必须添加
builder.Services.AddMemoryCache();

#region 添加微信配置（一行代码）

//Senparc.Weixin 注册（必须）
builder.Services.AddSenparcWeixinServices(builder.Configuration);

#endregion

#region 添加 NeuChar AppStore 开发者信息

builder.Services.Configure<NeucharSetting>(builder.Configuration.GetSection("NeucharSetting"));

#endregion

var app = builder.Build();

#region 启用微信配置（一句代码）

//手动获取配置信息可使用以下方法
//var senparcWeixinSetting = app.Services.GetService<IOptions<SenparcWeixinSetting>>()!.Value;

//启用微信配置（必须）
var registerService = app.UseSenparcWeixin(app.Environment,
    null /* 不为 null 则覆盖 appsettings  中的 SenpacSetting 配置*/,
    null /* 不为 null 则覆盖 appsettings  中的 SenpacWeixinSetting 配置*/,
    register => { /* CO2NET 全局配置 */ },
    (register, weixinSetting) =>
    {
        //注册公众号信息（可以执行多次，注册多个公众号）
        register.RegisterMpAccount(weixinSetting, "【盛派网络小助手】公众号");
    });

#region 使用 MessageHadler 中间件，用于取代创建独立的 Controller

//MessageHandler 中间件介绍：https://www.cnblogs.com/szw/p/Wechat-MessageHandler-Middleware.html
//使用公众号的 MessageHandler 中间件（不再需要创建 Controller）
app.UseMessageHandlerForMp("/WeixinAsync", AppMessageHandler.GenerateMessageHandler, options =>
{
    options.AccountSettingFunc = context => Senparc.Weixin.Config.SenparcWeixinSetting;
});

#endregion

#endregion

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
