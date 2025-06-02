using Common.Enums;

namespace DTO;

public static class Rfi2Dto
{
    public sealed record QueryIn
    {
        public AiModelType AiModelType { get; init; }

        public string Query { get; init; }
    }

    public sealed record QueryOut
    {
        public string Text { get; init; }
    }
}
