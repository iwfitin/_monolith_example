using DTO;
using Refit;

namespace BLL.Services;

public interface IChromaDb
{
    [Post("/clear")]
    Task<HttpResponseMessage> Clear();

    [Post("/add-text")]
    Task<HttpResponseMessage> AddText(ChromaDbDto.AddTextIn dto);
    
    [Multipart]
    [Post("/upload-file")]
    Task<HttpResponseMessage> UploadFile([AliasAs("source")] string source, [AliasAs("file")] StreamPart file);

    [Post("/query")]
    Task<HttpResponseMessage> Query(ChromaDbDto.QueryIn dto, CancellationToken cancellationToken);

    [Delete("/{id}/delete")]
    Task<HttpResponseMessage> Delete(string id);

    [Multipart]
    [Post("/file-meta-info")]
    Task<HttpResponseMessage> FileMetaInfo([AliasAs("file")] StreamPart file);
}
