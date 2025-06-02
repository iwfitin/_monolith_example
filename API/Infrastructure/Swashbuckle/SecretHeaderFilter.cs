using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Infrastructure.Swashbuckle;

internal sealed class SecretHeaderFilter : IOperationFilter
{
    private IDictionary<string, string> RouteToHeader { get; }

    public SecretHeaderFilter(IDictionary<string, string> routeToHeader)
    {
        RouteToHeader = routeToHeader;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var route = Route(context.MethodInfo.DeclaringType?.Name, context.MethodInfo.Name);
        if (!RouteToHeader.ContainsKey(route))
            return;

        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = RouteToHeader[route],
            In = ParameterLocation.Header,
            Schema = new OpenApiSchema
            {
                Type = "String",
                Default = new OpenApiString("x"),
            },
        });
    }

    public static string Route(string controllerName, string methodName)
    {
        return $"{controllerName}/{methodName}";
    }
}
