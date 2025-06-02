using DTO;
using Refit;

namespace BLL.Services;

public interface IOpenAi
{
    [Post("/completions")]
    Task<HttpResponseMessage> Completion([Authorize] string accessToken, OpenAiDto.Completion.Request request, CancellationToken cancellationToken);

    [Post("/chat/completions")]
    Task<HttpResponseMessage> ChatCompletion([Authorize] string accessToken, OpenAiDto.ChatCompletion.Request request, CancellationToken cancellationToken);
}
