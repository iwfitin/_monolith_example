using System.Net;
using System.Net.Mime;
using API.Extensions;
using Common.Extensions;
using Common.Static;
using DTO.Infrastructure;
using Microsoft.IO;

namespace API.Infrastructure.Middleware;

public sealed class ExceptionHandler
{
    private static int BodyLengthLimitInBytes => 42000;

    private RequestDelegate Next { get; }

    private ILogger Logger { get; }

    private RecyclableMemoryStreamManager MemoryStreamManager { get; }

    private static HashSet<string> LogMethods { get; } = new()
    {
        "POST",
        "PUT",
        "PATCH",
        "DELETE",
#if DEBUG
        "GET",
#else
#endif
    };

    private static HashSet<string> ExcludePaths { get; } = new();

    public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
    {
        Next = next;
        Logger = logger;
        MemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            if (LogMethods.Contains(context.Request.Method) && !ExcludePaths.Contains(context.Request.Path))
            {
                var id = RandomString.UpperAlpha(5);
                await LogRequest(context, id);
                await LogResponse(context, id);
            }
            else
                await Next(context);
        }
        catch (ArgumentException ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            var dto = new BadRequestDto
            {
                Errors = new Dictionary<string, IEnumerable<string>>
                {
                    {"error", new[] {ex.JoinInnerExceptions().CutExceptionCode(), }},
                },
                DevCode = ex.Message.ExceptionCode(),
            };
            await context.Response.WriteAsync(dto.ToJson());
            Logger.LogError(ex.JoinInnerExceptions());
        }
        catch (Exception ex)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var dto = new BadRequestDto
            {
                Errors = new Dictionary<string, IEnumerable<string>>
                {
                    {"error", new[] {ex.JoinInnerExceptions(), }},
                },
            };
            await context.Response.WriteAsync(dto.ToJson());
            Logger.LogError(ex.JoinInnerExceptions());
        }
    }

    private async Task LogRequest(HttpContext context, string id)
    {
        var request = new LogRequestDto
        {
            Method = context.Request.Method,
            Path = context.Request.Path,
            Query = context.Request.QueryString.ToString(),
            BodyJson = await RequestBodyJson(context),
            Headers = context.Request.Headers.ToDict(),
        };

        Logger.LogInformation($"_{nameof(request)}({id}): {request.ToJson()}");
    }

    private async Task<string> RequestBodyJson(HttpContext context)
    {
        context.Request.EnableBuffering();
        if (context.Request.ContentLength > BodyLengthLimitInBytes)
            return new DefaultBodyDto
            {
                Length = context.Request.ContentLength,
            }.ToJson();

        await using var requestStream = MemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestStream);
        context.Request.Body.Position = 0;

        return requestStream.ReadStreamInChunksToStr();
    }

    private async Task LogResponse(HttpContext context, string id)
    {
        var origin = context.Response.Body;
        await using var responseBody = MemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        try
        {
            await Next(context);

            var response = new LogResponseDto
            {
                StatusCode = context.Response.StatusCode,
                UserName = context.User.Identity?.Name,
                BodyJson = await ResponseBodyJson(context),
            };
            Logger.LogInformation($"_{nameof(response)}({id}): {response.ToJson()}");
            await responseBody.CopyToAsync(origin);
        }
        finally
        {
            context.Response.Body = origin;
        }
    }

    private async Task<string> ResponseBodyJson(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        if (context.Response.Body.Length > BodyLengthLimitInBytes)
            return new DefaultBodyDto
            {
                Length = context.Response.Body.Length,
            }.ToJson();

        var bodyJson = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        return bodyJson;
    }
}
