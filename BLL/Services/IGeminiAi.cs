using DTO;
using Refit;

namespace BLL.Services;

interface IGeminiAi
{
    [Post("/gemini-pro:generateContent?key={key}")]
    Task<HttpResponseMessage> GenerateText(string key, GeminiAiDto.Request request, CancellationToken cancellationToken);
}
