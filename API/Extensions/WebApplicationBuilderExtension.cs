using API.Infrastructure.Middleware;

namespace API.Extensions;

internal static class WebApplicationBuilderExtension
{
    internal static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        services.RegisterCors(configuration);
        services.AddControllers(x => x.Filters.Add<NoContentFilter>());
        services.AddSignalR();
        services.AddHttpContextAccessor();

        services.RegisterOptions(configuration);
        services.RegisterAppDbContext(configuration);
        services.RegisterAuth();
        services.RegisterJwtAuthorization(configuration);

        services.RegisterServices();

        services.RegisterRedis(configuration);
        services.RegisterSwagger();
    }

    internal static WebApplication Configure(this WebApplicationBuilder builder)
    {
        var app = builder.Build();
        var env = builder.Environment;
        var configuration = builder.Configuration;

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseMiddleware<ExceptionHandler>();

        app.UseHttpsRedirection();
        app.RegisterVirtualDir(configuration);

        app.UseRouting();
        app.UseCors();

        app.UseAuthentication();
        app.UseAuthorization();

        app.RegisterSwaggerUi();

        app.UseEndpoints(x => x.MapControllers());

        return app;
    }
}
