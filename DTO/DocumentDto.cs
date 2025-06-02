using System.ComponentModel.DataAnnotations;
using Common.Interfaces;

namespace DTO;

public static class DocumentDto
{
    public sealed record UploadFile<T>
    {
        [Required]
        public T File { get; init; }

        [Required]
        public DateTime LastModifiedTime { get; init; }
    }

    public sealed record Add
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string Path { get; init; }

        public DateTime LastModifiedTime { get; init;}
    }

    public sealed record List
    {
        public string Id { get; init; }

        public string Name { get; init; }

        public string Path { get; init; }

        public DateTime LastModifiedTime { get; init; }
    }

    public sealed record ByHasId : IHasId<string>
    {
        public string Id { get; set; }

        public string Name { get; init; }

        public string Path { get; init; }

        public DateTime LastModifiedTime { get; init; }
    }

    public sealed record FileMetaInfo<T>
    {
        [Required]
        public T File { get; init; }
    }
}
