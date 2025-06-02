using System.Text.Json.Serialization;

namespace DTO;

public static class ChromaDbDto
{
    public interface IMetadata
    {
        string Source { get; init; }
    }

    public sealed record AddTextIn: IMetadata
    {
        public string Text { get; init; }

        public string Source { get; init; }
    }

    public sealed record QueryIn
    {
        public string Query { get; init; }

        [JsonPropertyName("n_results")]
        public int CountOfResults { get; init; }

        public string Source { get; init; }
    }

    public sealed record QueryOut
    {
        public string Text { get; init; }

        public decimal Distance { get; init; }

        public Metadata Metadata { get; init; }
    }

    public sealed record Metadata : IMetadata
    {
        public string Source { get; init; }

        public string Name { get; init; }

        public DateTime LastModifiedTime { get; init; }
    }

    public sealed record FileMetaInfoOut
    {
        public string Creator { get; init; }

        public DateTime Created { get; init; }

        public string LastModifiedBy { get; init; }

        public DateTime Modified { get; init; }
    }
}
