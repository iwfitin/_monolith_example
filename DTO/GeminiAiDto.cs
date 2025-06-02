namespace DTO;

public static class GeminiAiDto
{
    public sealed record Request
    {
        public ICollection<Content> Contents { get; init; }
    }

    public sealed record Content
    {
        public ICollection<Part> Parts { get; init; }
    }

    public sealed record Part
    {
        public string Text { get; init; }
    }

    public sealed record Response
    {
        public ICollection<Candidate> Candidates { get; init; }
    }

    public sealed record Candidate
    {
        public Content Content { get; init; }

        public string FinishReason { get; init; }
    }
}
