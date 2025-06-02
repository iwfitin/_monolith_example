using System.Text;
using System.Text.RegularExpressions;
using Common.Extensions;
using Common.Static;
using DTO;
using DTO.Infrastructure;
using Mapster;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace BLL.Services;

public sealed class CompletionService
{
    private ILogger Logger { get; }

    private SettingsDto.Completion Settings { get; }

    private ChromaDbService ChromaDbService { get; }

    public CompletionService(ILogger<CompletionService> logger, IOptions<SettingsDto.Completion> settings, ChromaDbService chromaDbService)
    {
        Logger = logger;
        Settings = settings.Value;
        ChromaDbService = chromaDbService;
    }

    public async Task<CompletionDto.QueryOut> Query(CompletionDto.QueryIn dto)
    {
        var contexts = await Contexts(dto.Query);
        var api = RestService.For<IOpenAi>(Settings.Url);
        var answer = await FirstAnswer(api, contexts.FirstOrDefault(), dto.Query);

        const string template =
            "<answer>" +
            "\n---------------------\n" +
            "We have the opportunity to refine the above answer " +
            "(only if needed) with some more context below.\n" +
            "---------------------\n" +
            "<context>" +
            "\n---------------------\n" +
            "Given the new context, refine the original answer to better " +
            "answer the question: <query>" +
            "If the context isn't useful, output the original answer again.";

        foreach (var x in contexts.Skip(1))
        {
            var dict = new Dictionary<string, string>
            {
                { "<answer>", answer },
                { "<context>", x },
                { "<query>",  dto.Query },
            };
            var prompt = Regex.Replace(template, @"<\w+>", x => dict[x.Groups[0].Value]);
            var id = RandomString.UpperAlpha(5);
            Logger.LogInformation($"_to_openai({id}): {prompt}");

            var response = await Troy.Try(
                async () =>
                    await api.Completion(Settings.AccessToken, BuildRequest(prompt), new CancellationTokenSource(Settings.TimeoutInMilliseconds).Token),
                ex =>
                {
                    if (ex is TaskCanceledException or HttpRequestException)
                        throw new ArgumentException($"2521. openai with endpoint {Settings.Url} does not answer");
                    throw new ArgumentException($"2522. {ex.JoinInnerExceptions()}");
                });

            var json = await response.Content.ReadAsStringAsync();
            Logger.LogInformation($"_from_openai({id}): {json}");
            if (!response.IsSuccessStatusCode)
                throw new ArgumentException($"2509. openai error: {json}");

            answer = json.FromJson<OpenAiDto.Completion.Response>().Choices.FirstOrDefault()?.Text ?? answer;
        }

        return new CompletionDto.QueryOut
        {
            Text = answer,
        };
    }

    private async Task<ICollection<string>> Contexts(string query)
    {
        var contexts = await ChromaDbService.Query(new ChromaDbDto.QueryIn
        {
            Query = query,
            CountOfResults = Settings.CountOfResults,
        });

        var lst = new List<string>();
        var str = new StringBuilder();
        foreach (var x in contexts)
        {
            if (str.Length + x.Text.Length > Settings.PromptLengthLimit)
            {
                lst.Add(str.ToString());
                str.Clear();
            }
            str.Append($"{x.Text}\n");
        }
        lst.Add(str.ToString());

        return lst;
    }

    private async Task<string> FirstAnswer(IOpenAi api, string context, string query)
    {
        const string template =
            "Context information is below. \n" +
            "---------------------\n" +
            "<context>" +
            "\n---------------------\n" +
            "Given the context information and no prior knowledge, " +
            "answer the question: <query>\n";

        var dict = new Dictionary<string, string>
        {
            { "<context>", context },
            { "<query>",  query },
        };
        var prompt = Regex.Replace(template, @"<\w+>", x => dict[x.Groups[0].Value]);
        var id = RandomString.UpperAlpha(5);
        Logger.LogInformation($"_to_openai({id}): {prompt}");

        var response = await Troy.Try(
            async () =>
                await api.Completion(Settings.AccessToken, BuildRequest(prompt), new CancellationTokenSource(Settings.TimeoutInMilliseconds).Token),
            ex =>
            {
                if (ex is TaskCanceledException or HttpRequestException)
                    throw new ArgumentException($"2523. openai with endpoint {Settings.Url} does not answer");
                throw new ArgumentException($"2524. {ex.JoinInnerExceptions()}");
            });

        var json = await response.Content.ReadAsStringAsync();
        Logger.LogInformation($"_from_openai({id}): {json}");
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"2505. openai error: {json}");

        return json.FromJson<OpenAiDto.Completion.Response>().Choices.FirstOrDefault()?.Text;
    }

    private OpenAiDto.Completion.Request BuildRequest(string prompt)
    {
        return Settings.Adapt<OpenAiDto.Completion.Request>() with
        {
            Prompt = prompt
        };
    }
}
