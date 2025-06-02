using Common.Extensions;
using Common.Static;
using DTO;
using DTO.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace BLL.Services;

public sealed class GeminiAiService
{
    private ILogger Logger { get; }

    private SettingsDto.GeminiAi Settings { get; }

    public GeminiAiService(ILogger<ChatCompletionService> logger, IOptions<SettingsDto.GeminiAi> settings)
    {
        Logger = logger;
        Settings = settings.Value;
    }

    public async Task<string> Request(string text)
    {
        var api = RestService.For<IGeminiAi>(Settings.Url);
        var dto = new GeminiAiDto.Request
        {
            Contents = new List<GeminiAiDto.Content>
            {
                new()
                {
                    Parts = new List<GeminiAiDto.Part>
                    {
                        new()
                        {
                            Text = text,
                        }
                    }
                }
            }
        };
        var id = RandomString.UpperAlpha(5);
        Logger.LogInformation($"_to_gemini_ai({id}): {dto.ToJson()}");

        var response = await Troy.Try(
        async () =>
            await api.GenerateText(Settings.Key, dto, new CancellationTokenSource(Settings.TimeoutInMilliseconds).Token),
            ex =>
            {
                if (ex is TaskCanceledException or HttpRequestException)
                    throw new ArgumentException($"2527. openai with endpoint {Settings.Url} does not answer");
                throw new ArgumentException($"2528. {ex.JoinInnerExceptions()}");
            });

        var json = await response.Content.ReadAsStringAsync();
        Logger.LogInformation($"_from_gemini_ai({id}): {json}");
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"2506. gemini error: {json}");

        var responseDto = json.FromJson<GeminiAiDto.Response>();
        if (!responseDto.Candidates.Any())
            throw new ArgumentException($"2513. violence question: {json}");

        return responseDto.Candidates.FirstOrDefault()?.Content.Parts.FirstOrDefault()?.Text;
    }
}
