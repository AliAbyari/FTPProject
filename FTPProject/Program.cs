using FTPProject.Domain.DTOs;
using FTPProject.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Net;

ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

var builder = WebApplication.CreateBuilder(args);
#region SeriLog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();
Log.Logger.Information("Starting up");
#endregion

builder.Services.AddControllers();
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSetting>(appSettingsSection);

var appSettings = appSettingsSection.Get<AppSetting>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("application", new OpenApiInfo { Title = "Application API", Version = "1.01" });
    var filePath = Path.Combine(System.AppContext.BaseDirectory, "FTPProject.xml");
    c.IncludeXmlComments(filePath, true);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });
    c.UseInlineDefinitionsForEnums();
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
});

builder.Services.Configure<AppSettings>(builder.Configuration);

builder.Services.AddSingleton<FtpService>();

#region SeriLog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        $@"{appSettings.LogFilePath}{appSettings.LogFileName}.txt",
        rollingInterval: (RollingInterval.Day),
        fileSizeLimitBytes: (appSettings.LogFileSizeMB * 1000000),
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: null)
    .CreateLogger();
#endregion
var app = builder.Build();


app.UseSwagger(c =>
{
    c.PreSerializeFilters.Add((swaggerDoc, httpReq) => httpReq.Scheme = httpReq.Host.Value);
});
var str = app.Configuration["SwaggerAddress"];
app.UseSwaggerUI(c => { c.SwaggerEndpoint(app.Configuration["SwaggerAddress"], "Application API 1.01"); c.InjectStylesheet(app.Configuration["SwaggerCssAddress"]); }); app.UseStaticFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
