using Common.Extensions;
using DAL.EF;
using DTO.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace API.Extensions;

internal static class ApplicationBuilderExtension
{
    internal static void RegisterVirtualDir(this IApplicationBuilder app, IConfiguration configuration)
    {
        var settings = configuration.GetSection(nameof(SettingsDto.VirtualDir)).Get<SettingsDto.VirtualDir>();
        settings.Dir.CreateDirectoryIfNotExist();
        app.UseFileServer(new FileServerOptions
        {
            FileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory().Combine(settings.Dir)),
            RequestPath = new PathString(settings.Url),
        });
    }

    internal static void RegisterSwaggerUi(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/swagger/v1/swagger.json", "openai-integration api v1");
            x.RoutePrefix = string.Empty;
            x.DefaultModelExpandDepth(3);
            x.DefaultModelRendering(ModelRendering.Example);
            x.DefaultModelsExpandDepth(-1);
            x.DisplayOperationId();
            x.DisplayRequestDuration();
            x.DocExpansion(DocExpansion.None);
            x.EnableDeepLinking();
            x.EnableFilter();
            x.ShowExtensions();
        });
    }

    internal static void AutoMigrateDb(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        context.Database.Migrate();
    }
}
