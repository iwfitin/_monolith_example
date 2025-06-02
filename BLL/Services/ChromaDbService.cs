using BLL.Base;
using Common.Extensions;
using Common.Static;
using DAL.EF;
using DAL.Entities.Files;
using DTO;
using DTO.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace BLL.Services;

public sealed class ChromaDbService : ContextHasService
{
    private static ISet<string> SupportedFileExtensions { get; }

    static ChromaDbService()
    {
        SupportedFileExtensions = new HashSet<string>
        {
            ".docx",
            ".xlsx",
            ".pptx",
        };
    }

    private ILogger Logger { get; }

    private SettingsDto.ChromaDb Settings { get; }

    public ChromaDbService(ILogger<ChromaDbService> logger, AppDbContext context, IOptions<SettingsDto.ChromaDb> settings) : base(context)
    {
        Logger = logger;
        Settings = settings.Value;
    }

    public async Task Upload(string id, (Stream Source, string Name) file)
    {
        var clone = await file.Source.Clone();
        var api = RestService.For<IChromaDb>(Settings.Url);
        var response = await api.UploadFile(id, new StreamPart(clone, file.Name));
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"2500. chroma api error: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<ICollection<ChromaDbDto.QueryOut>> Query(ChromaDbDto.QueryIn dto)
    {
        var api = RestService.For<IChromaDb>(Settings.Url);
        var id = RandomString.UpperAlpha(5);
        Logger.LogInformation($"_to_chroma_db({id}): {dto.ToJson()}");

        var response = await Troy.Try(
            async () => 
                await api.Query(dto, new CancellationTokenSource(Settings.TimeoutInMilliseconds).Token),
        ex =>
            {
                if (ex is TaskCanceledException or HttpRequestException)
                    throw new ArgumentException($"2515. chromadb with endpoint {Settings.Url} does not answer");
                throw new ArgumentException($"2516. {ex.JoinInnerExceptions()}");
            });

        var json = await response.Content.ReadAsStringAsync();
        Logger.LogInformation($"_from_chroma_db({id}): {json}");
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"2501. chroma api error: {json}");

        var dict = await Context.Set<Document>()
            .ToDictionaryAsync(x => x.Id, x => new
            {
                x.Name,
                x.LastModifiedTime,
            });

        return json.FromJson<ICollection<ChromaDbDto.QueryOut>>()
            .Select(x => x with
            {
                Metadata = new ChromaDbDto.Metadata
                {
                    Source = x.Metadata.Source,
                    Name = dict[x.Metadata.Source].Name,
                    LastModifiedTime = dict[x.Metadata.Source].LastModifiedTime,
                }
            })
            .ToList();
    }

    public async Task Delete(string id)
    {
        var api = RestService.For<IChromaDb>(Settings.Url);
        var response = await api.Delete(id);
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"2504. chroma api error: {await response.Content.ReadAsStringAsync()}");
    }

    public async Task<ChromaDbDto.FileMetaInfoOut> FileMetaInfo((Stream Source, string Name) file)
    {
        if (!SupportedFileExtensions.Contains(file.Name.FileExtension()))
            return new ChromaDbDto.FileMetaInfoOut
            {
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
            };

        var clone = await file.Source.Clone();
        var api = RestService.For<IChromaDb>(Settings.Url);
        var response = await api.FileMetaInfo(new StreamPart(clone, file.Name));
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException($"2508. chroma api error: {json}");

        return json.FromJson<ChromaDbDto.FileMetaInfoOut>();
    }
}
