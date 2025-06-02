namespace DTO.Infrastructure;

public static class SettingsDto
{
    public sealed record Openai
    {
        public Completion Completion { get; init; }

        public ChatCompletion ChatCompletion { get; init; }
    }

    public sealed record Completion
    {
        public string Url { get; init; }

        public string AccessToken { get; init; }

        public int TimeoutInMilliseconds { get; init; }

        public int PromptLengthLimit { get; init; }

        public int CountOfResults { get; init; }

        public string Model { get; init; }

        public decimal Temperature { get; init; }

        public int MaxTokens { get; init; }

        public decimal FrequencyPenalty { get; init; }
    }

    public sealed record ChatCompletion
    {
        public string Url { get; init; }

        public string AccessToken { get; init; }

        public int TimeoutInMilliseconds { get; init; }

        public string Model { get; init; }

        public decimal Temperature { get; init; }

        public int MaxTokens { get; init; }

        public decimal FrequencyPenalty { get; init; }
    }

    public sealed record GeminiAi
    {
        public string Url { get; init; }

        public string Key { get; init; }

        public int TimeoutInMilliseconds { get; init; }
    }

    public sealed record ChromaDb
    {
        public string Url { get; init; }

        public int TimeoutInMilliseconds { get; init; }
    }

    public sealed record VirtualDir
    {
        public string Dir { get; init; }

        public string Url { get; init; }
    }

    public sealed record Cors
    {
        public IEnumerable<string> Origins { get; init; }
    }
}
