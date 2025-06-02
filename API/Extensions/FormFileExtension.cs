namespace API.Extensions;

internal static class FormFileExtension
{
    internal static async Task<(Stream Source, string FileName)> ToStream(this IFormFile file)
    {
        if (file is null)
            throw new ArgumentException("2526. The file is not attached", nameof(file));

        var source = new MemoryStream();
        await file.CopyToAsync(source);

        return (source, file.FileName);
    }
}
