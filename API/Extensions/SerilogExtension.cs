using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Text.RegularExpressions;

namespace API.Extensions;

internal static class SerilogExtension
{
    internal static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        var logging = builder.Logging;
        logging.ClearProviders();

        var connectionString = builder.Configuration.GetConnectionString("Default").ToLoggingDbConnStr();
        var columnOptions = new ColumnOptions
        {
            TimeStamp = { ConvertToUtc = true, ColumnName = "CreatedTimeUtc", }
        };
        columnOptions.Store.Remove(StandardColumn.MessageTemplate);
        columnOptions.Store.Remove(StandardColumn.Properties);
        logging.AddSerilog(new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Error)
            .MinimumLevel.Override("Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware", LogEventLevel.Information)
            .WriteTo.MSSqlServer(
                connectionString: connectionString,
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    AutoCreateSqlDatabase = true,
                    AutoCreateSqlTable = true,
                },
                columnOptions: columnOptions)
            .CreateLogger());
    }

    private static string ToLoggingDbConnStr(this string str)
    {
        return Regex.Replace(str,
            @"Database=(.*?);", @"Database=$1_log;");
    }
}
