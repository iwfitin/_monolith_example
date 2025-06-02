using BLL.Base;
using Common.Extensions;
using Common.Static;
using DAL.EF;
using DAL.Entities.Files;
using DTO;
using DTO.Infrastructure;
using Microsoft.Extensions.Options;

namespace BLL.Services;

public sealed class DocumentService : EntityService<Document, string>
{
    private static Dictionary<string, string> MediaTypeNames { get; }

    static DocumentService()
    {
        MediaTypeNames = new Dictionary<string, string>
        {
            { ".csv", "text/csv" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".ppt", "application/vnd.ms-powerpoint" },
            { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { ".txt", "text/plain" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".xml", "application/xml" },
        };
    }

    private SettingsDto.VirtualDir Settings { get; }

    private ChromaDbService ChromaDbService { get; }

    public DocumentService(AppDbContext context, IOptions<SettingsDto.VirtualDir> settings, ChromaDbService chromaDbService) : base(context)
    {
        Settings = settings.Value;
        ChromaDbService = chromaDbService;
    }

    public async Task<string> Upload((Stream Source, string Name) file, DateTime lastModifiedTime)
    {
        var id = Guid.NewGuid().ToString("N");
        await ChromaDbService.Upload(id, file);

        var path = Settings.Dir.Combine($"{RandomString.LowerAlpha(5)}_{file.Name}");
        await (path, file.Source).SaveStreamByPath();
        var dto = new DocumentDto.Add
        {
            Id = id,
            Name = file.Name,
            Path = path,
            LastModifiedTime = lastModifiedTime,
        };

        return (await Add(dto)).Id;
    }

    public async Task<(byte[] FileContents, string ContentType, string FileDownloadName)> Download(string id)
    {
        var path = await Select(id, x => x.Path);
        var ext = path.FileExtension();
        if (!MediaTypeNames.ContainsKey(ext))
            throw new ArgumentException($"2507. extension {ext} not supported.");

        return (await File.ReadAllBytesAsync(path), MediaTypeNames[ext], $"{DateTime.UtcNow:MM_dd-HH_mm}");
    }

    public override async Task Delete(string id)
    {
        await ChromaDbService.Delete(id);
        await base.Delete(id);
    }
}
