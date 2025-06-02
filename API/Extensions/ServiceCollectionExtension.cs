using API.Infrastructure.Swashbuckle;
using BLL.Services;
using BLL.Static;
using BLL.Users;
using DAL.EF;
using DAL.Entities.Users;
using DTO.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace API.Extensions;

internal static class ServiceCollectionExtension
{
    internal static void RegisterCors(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(nameof(SettingsDto.Cors)).Get<SettingsDto.Cors>();
        if (settings is null)
            throw new ArgumentException($"2558. cors settings not found");

        services.AddCors(x => x.AddDefaultPolicy(x => x
            .WithOrigins(settings.Origins.ToArray())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));
    }

    internal static void RegisterOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthDto.Jwt>(x => configuration.GetSection(nameof(AuthDto.Jwt)).Bind(x));
        services.Configure<SettingsDto.VirtualDir>(x => configuration.GetSection(nameof(SettingsDto.VirtualDir)).Bind(x));
        services.Configure<SettingsDto.Completion>(x => configuration.GetSection($"{nameof(SettingsDto.Openai)}:{nameof(SettingsDto.Completion)}").Bind(x));
        services.Configure<SettingsDto.ChatCompletion>(x => configuration.GetSection($"{nameof(SettingsDto.Openai)}:{nameof(SettingsDto.ChatCompletion)}").Bind(x));
        services.Configure<SettingsDto.GeminiAi>(x => configuration.GetSection(nameof(SettingsDto.GeminiAi)).Bind(x));
        services.Configure<SettingsDto.ChromaDb>(x => configuration.GetSection(nameof(SettingsDto.ChromaDb)).Bind(x));
    }

    internal static void RegisterAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("Default")));
    }

    internal static void RegisterAuth(this IServiceCollection services)
    {
        services.AddIdentityCore<AspNetUser>(x =>
            {
                x.Password.RequiredLength = 1;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequireDigit = false;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();
    }

    internal static void RegisterJwtAuthorization(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = JwtBuilder.Parameters(configuration);
            });
        services.AddAuthorizationCore();
    }

    internal static void RegisterServices(this IServiceCollection services)
    {
        services.AddTransient<PasswordHasher<AspNetUser>>();
        services.AddTransient<DocumentService>();
        services.AddTransient<ChromaDbService>();
        services.AddTransient<RoleService>();
        services.AddTransient<AspNetUserService>();
        services.AddTransient<UserService>();
        services.AddTransient<AdminService>();
        services.AddTransient<AuthService>();
        services.AddTransient<CompletionService>();
        services.AddTransient<ChatCompletionService>();
        services.AddTransient<GeminiAiService>();
        services.AddTransient<Rfi2Service>();
    }

    internal static void RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "monolith-example api",
                Version = "v1",
            });
            x.DescribeAllParametersInCamelCase();

            x.AddSecurityDefinition(JwtBuilder.Bearer(), new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter JWT with Bearer into field",
                Name = JwtBuilder.Authorization(),
                Type = SecuritySchemeType.ApiKey,
                Scheme = JwtBuilder.Bearer(),
            });

            x.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBuilder.Bearer(),
                        },
                        Scheme = "oauth2",
                        Name = JwtBuilder.Bearer(),
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                },
            });
            x.SchemaFilter<DtoExampleFilter>();

            x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{nameof(API)}.xml"));
            x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{nameof(DTO)}.xml"));

            x.CustomSchemaIds(x => x.FullName?.Replace("+", "."));
        });
    }

    internal static void RegisterRedis(this IServiceCollection services, IConfiguration configuration, string connectionStringName = "Redis")
    {
        services.AddStackExchangeRedisCache(x => x.Configuration = configuration.GetConnectionString(connectionStringName));
    }

    internal static void RegisterMemoryCache(this IServiceCollection services, IConfiguration configuration, string connectionStringName = "Redis")
    {
        services.AddMemoryCache();
    }
}
