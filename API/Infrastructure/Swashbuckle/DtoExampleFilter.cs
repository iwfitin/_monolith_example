using System.Reflection;
using System.Runtime.Serialization;
using Common.Extensions;
using Common.Static;
using DTO.Infrastructure;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Infrastructure.Swashbuckle;

internal sealed class DtoExampleFilter : ISchemaFilter
{
    #region StaticProperties
    private static int DefaultInt => 1;

    private static double DefaultDouble => 2.72;

    private static int _counter;

    private static int Counter => ++_counter;
    #endregion

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        schema.Example = schema.Type switch
        {
            "string" when context.ParameterInfo is not null => new OpenApiNull(),
            "string" when context.MemberInfo?.Name == "VenueId" => new OpenApiString("ab30f50861bf4fb5bb183703acf0cac6"),
            "string" when schema.Format == "date-time" => new OpenApiString(RandomDateTime.Minutes().ToString("yyyy-MM-ddTHH:mm:ssZ")),
            "string" => new OpenApiString($"_string{Counter}"),
            "integer" when context.ParameterInfo is not null => new OpenApiNull(),
            "integer" => new OpenApiInteger(DefaultInt),
            "decimal" => new OpenApiDouble(DefaultDouble),
            _ => schema.Example,
        };

        foreach (var x in IgnoreProperties(context.Type))
            schema.Properties.Remove(x);

        foreach (var (camel, snake) in Properties(context.Type))
        {
            if (!schema.Properties.ContainsKey(camel) || !snake.Contains('_'))
                continue;

            var apiSchema = schema.Properties[camel];
            schema.Properties.Remove(camel);
            schema.Properties.Add(snake, apiSchema);
        }

        if (context.Type == typeof(AuthDto.Login))
            schema.Example = new OpenApiObject
            {
                [$"{nameof(AuthDto.Login.UserName)}".ToCamelCase()] = new OpenApiString("admin@monolith-example.com"),
                [$"{nameof(AuthDto.Login.Password)}".ToCamelCase()] = new OpenApiString("x"),
            };

        else
            schema.Example = schema.Example;
    }

    private IEnumerable<string> IgnoreProperties(Type type)
    {
        foreach (var x in type.GetProperties())
        {
            var attr = x.GetCustomAttribute<IgnoreDataMemberAttribute>();
            if (attr is null)
                continue;

            yield return x.Name.ToCamelCase();
        }
    }

    private IEnumerable<(string Camel, string Snake)> Properties(Type type)
    {
        foreach (var x in type.GetProperties())
        {
            var attr = x.GetCustomAttribute<DataMemberAttribute>();
            if (attr is null)
                continue;

            yield return (x.Name.ToCamelCase(), attr.Name);
        }
    }
}
