using Common.Extensions;
using Common.Static;
using DTO;
using DTO.Infrastructure;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace BLL.Services;

public sealed class ChatCompletionService
{
    private ILogger Logger { get; }

    private SettingsDto.ChatCompletion Settings { get; }

    public ChatCompletionService(ILogger<ChatCompletionService> logger, IOptions<SettingsDto.ChatCompletion> settings)
    {
        Logger = logger;
        Settings = settings.Value;
    }

    internal async Task<string> Request(OpenAiDto.ChatCompletion.Request dto)
    {
        var api = RestService.For<IOpenAi>(Settings.Url);
        var id = RandomString.UpperAlpha(5);
        Logger.LogInformation($"_to_openai({id}): {dto.ToJson()}");

        var response = await Troy.Try(
            async () =>
                await api.ChatCompletion(Settings.AccessToken, dto, new CancellationTokenSource(Settings.TimeoutInMilliseconds).Token),
            ex =>
            {
                if (ex is TaskCanceledException or HttpRequestException)
                    throw new ArgumentException($"2517. openai with endpoint {Settings.Url} does not answer");
                throw new ArgumentException($"2519. {ex.JoinInnerExceptions()}");
            });

        var json = await response.Content.ReadAsStringAsync();
        Logger.LogInformation($"_from_openai({id}): {json}");
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"2512. openai error: {json}");

        return json.FromJson<OpenAiDto.ChatCompletion.Response>().Choices.FirstOrDefault()?.Message.Content;
    }

    internal OpenAiDto.ChatCompletion.Request BuildRequest(string systemContent, string userContent)
    {
        return Settings.Adapt<OpenAiDto.ChatCompletion.Request>() with
        {
            Messages = new List<OpenAiDto.ChatCompletion.Message>
            {
                new()
                {
                    Role = "system",
                    Content = systemContent,
                },
                new()
                {
                    Role = "user",
                    Content = userContent,
                },
            }
        };
    }
}
