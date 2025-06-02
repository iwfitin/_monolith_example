using System.Security.Cryptography;

namespace Common.Extensions;

public static class StreamExtension
{
    public static async Task SaveStreamByPath(this (string Path, Stream Source) x)
    {
        var file = x.Path.RecreateFile();

        x.Source.Seek(0, SeekOrigin.Begin);
        await x.Source.CopyToAsync(file);

        x.Source.Close();
        file.Close();
    }

    public static async Task<Stream> Clone(this Stream source)
    {
        var pos = source.Position;
        if (source.CanSeek)
            source.Seek(0, SeekOrigin.Begin);
        var destination = new MemoryStream();
        await source.CopyToAsync(destination);
        if (destination.CanSeek)
            destination.Seek(0, SeekOrigin.Begin);
        source.Position = pos;

        return destination;
    }

    public static byte[] ToBytes(this Stream stream)
    {
        var ms = new MemoryStream();
        stream.CopyTo(ms);

        return ms.ToArray();
    }

    public static string ReadStreamInChunksToStr(this Stream stream)
    {
        const int readChunkBufferLength = 4096;

        stream.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);

        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);
        stream.Seek(0, SeekOrigin.Begin);

        return textWriter.ToString();
    }

    public static async Task<Stream> ReadStreamInChunks(this Stream stream)
    {
        const int readChunkBufferLength = 4096;

        var memoryStream = new MemoryStream();
        var buffer = new byte[readChunkBufferLength];
        int bytesRead;

        while ((bytesRead = await stream.ReadAsync(buffer, 0, readChunkBufferLength)) > 0)
            await memoryStream.WriteAsync(buffer, 0, bytesRead);

        memoryStream.Seek(0, SeekOrigin.Begin);

        if (stream.CanSeek)
            stream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }

    public static string Sha256HexStr(this Stream payload)
    {
        return Convert.ToHexString(payload.Sha256());
    }

    public static byte[] Sha256(this Stream payload)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(payload);
        if (payload.CanSeek)
            payload.Seek(0, SeekOrigin.Begin);

        return hash;
    }
}
