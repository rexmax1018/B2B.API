using Autofac;
using Autofac.Extensions.DependencyInjection;
using B2B.API.Middlewares;
using B2B.API.Modules;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using System.Text;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Info("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Autofac注入
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterModule(new AutofacModuleRegister());
                });

    // Add services to the container.
    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        // 當驗證失敗時，回應標頭會包含 WWW-Authenticate 標頭，這裡會顯示失敗的詳細錯誤原因
                        options.IncludeErrorDetails = true; // 預設值為 true，有時會特別關閉

                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            // 透過這項宣告，就可以從 "sub" 取值並設定給 User.Identity.Name
                            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                            // 透過這項宣告，就可以從 "roles" 取值，並可讓 [Authorize] 判斷角色
                            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",

                            // 一般我們都會驗證 Issuer
                            ValidateIssuer = true,
                            ValidIssuer = builder.Configuration.GetValue<string>("JwtSettings:Issuer"),

                            // 通常不太需要驗證 Audience
                            ValidateAudience = false,
                            //ValidAudience = "JwtAuthDemo", // 不驗證就不需要填寫

                            // 一般我們都會驗證 Token 的有效期間
                            ValidateLifetime = true,

                            // 如果 Token 中包含 key 才需要驗證，一般都只有簽章而已
                            ValidateIssuerSigningKey = false,

                            // "1234567890123456" 應該從 IConfiguration 取得
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JwtSettings:SignKey")))
                        };
                    });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // Log response info (for response pipeline: after ExceptionMiddleware)
    app.UseResponseLogMiddleware();

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    // Log request info (for request pipeline: after Routing)
    app.UseRequestLogMiddleware();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}