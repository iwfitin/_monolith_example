using Common.Enums;
using DTO;

namespace BLL.Services;

public sealed class Rfi2Service
{
    private static int CountOfResultsFromVectorDb { get; }

    private static string GiveAnswerInstruction { get;}

    static Rfi2Service()
    {
        CountOfResultsFromVectorDb = 10;

        GiveAnswerInstruction = "Answer a question related to our company (Kanda Software). The following information may be useful. But note that it may contain irrelevant information, in which case ignore this part. Use the voice of Kanda Software.";
    }

    private ChatCompletionService ChatCompletionService { get; }

    private ChromaDbService ChromaDbService { get; }

    private GeminiAiService GeminiAiService { get; }

    public Rfi2Service(ChatCompletionService chatCompletionService, ChromaDbService chromaDbService, GeminiAiService geminiAiService)
    {
        ChatCompletionService = chatCompletionService;
        ChromaDbService = chromaDbService;
        GeminiAiService = geminiAiService;
    }

    public async Task<Rfi2Dto.QueryOut> Query(Rfi2Dto.QueryIn dto)
    {
        var lst = new List<string>
        {
            await RetrieveFromKnowledgeDb(dto.Query, CountOfResultsFromVectorDb),
        };

        return new Rfi2Dto.QueryOut
        {
            Text = await GiveFinalAnswer(dto.Query, string.Join("\n", lst), dto.AiModelType),
        };
    }

    private async Task<string> RetrieveFromKnowledgeDb(string question, int countOfResults)
    {
        var lst = await ChromaDbService.Query(new ChromaDbDto.QueryIn
        {
            Query = question,
            CountOfResults = countOfResults,
        });

        return string.Join("\n", lst.Select(x => x.Text));
    }

    private async Task<string> GiveFinalAnswer(string question, string context, AiModelType modelType)
    {
        return modelType switch
        {
            AiModelType.OpenAi => await ChatCompletionService.Request(
                ChatCompletionService.BuildRequest(GiveAnswerInstruction,
                    $"question is:\n{question}\nthis is the context:\n{context}")),
            AiModelType.GeminiAi => await GeminiAiService.Request(
                $"Main question is: {question}\n\n{GiveAnswerInstruction}"),
            _ => string.Empty,
        };
    }
}
