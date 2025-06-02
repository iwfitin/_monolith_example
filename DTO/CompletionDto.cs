namespace DTO;

public static class CompletionDto
{
    public sealed record QueryIn
    {
        public string Query { get; init; }
    }

    public sealed record QueryOut
    {
        public string Text { get; init; }
    }
}
