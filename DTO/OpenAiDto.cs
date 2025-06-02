using System.Text.Json.Serialization;

namespace DTO;

public static class OpenAiDto
{
    public static class Completion
    {
        public sealed record Request
        {
            public string Model { get; init; }

            public string Prompt { get; init; }

            public decimal Temperature { get; init; }

            [JsonPropertyName("max_tokens")]
            public int MaxTokens { get; init; }

            [JsonPropertyName("frequency_penalty")]
            public decimal FrequencyPenalty { get; init; }
        }

        public sealed record Response
        {
            public ICollection<Choice> Choices { get; init; }
        }

        public sealed record Choice
        {
            public string Text { get; init; }
        }
    }

    public static class ChatCompletion
    {
        public sealed record Request
        {
            public string Model { get; init; }

            [JsonPropertyName("max_tokens")]
            public int MaxTokens { get; init; }

            public decimal Temperature { get; init; }

            [JsonPropertyName("frequency_penalty")]
            public decimal FrequencyPenalty { get; init; }

            public ICollection<Message> Messages { get; init; }
        }

        public sealed record Message
        {
            public string Role { get; init; }

            public string Content { get; init; }
        }

        public sealed record Response
        {
            public ICollection<Choice> Choices { get; init; }
        }

        public sealed record Choice
        {
            public Message Message { get; init; }
        }
    }
}
