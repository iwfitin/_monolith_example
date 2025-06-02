using System.Security.Cryptography;
using System.Web;

namespace Common.Extensions;

public static class StringLikePathExtension
{

    public static string ToLink(this string path)
    {
        return HttpUtility.UrlPathEncode(path.Replace('\\', '/'));
    }

    public static bool FileIsExist(this string path)
    {
        return File.Exists(path);
    }

    public static FileStream RecreateFile(this string path)
    {
        path.DeleteFileIfExist();
        path.FileDirectoryName().CreateDirectoryIfNotExist();
        var stream = File.Create(path);
        if (!File.Exists(path))
            throw new ArgumentException($"2575. failed to create file {path}");

        return stream;
    }

    public static void DeleteFileIfExist(this string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }

    public static long Size(this string path)
    {
        if (!File.Exists(path))
            return 0;

        return new FileInfo(path).Length;
    }

    public static DateTime CreationTime(this string path)
    {
        if (!File.Exists(path))
            return DateTime.MinValue;

        return new FileInfo(path).CreationTimeUtc;
    }

    public static byte[] FileSha256(this string path)
    {
        if (!File.Exists(path))
            throw new ArgumentException($"5512. file in path = '{path}' is not found");

        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(path);

        return sha256.ComputeHash(stream);
    }

    public static string PathFileName(this string path)
    {
        return !File.Exists(path) ? null : path.FileName();
    }

    public static string FileName(this string path)
    {
        return Path.GetFileName(path);
    }

    public static string FileExtension(this string path)
    {
        return Path.GetExtension(path);
    }

    public static string PathFileNameWithoutExtension(this string path)
    {
        return !File.Exists(path) ? null : path.FileNameWithoutExtension();
    }

    public static string FileDirectoryName(this string path)
    {
        return Path.GetDirectoryName(path);
    }

    public static string FileNameWithoutExtension(this string fileName)
    {
        return Path.GetFileNameWithoutExtension(fileName);
    }

    public static string Combine(this string path, params string[] suffixes)
    {
        foreach (var x in suffixes)
            path = Path.Combine(path, x);

        return path;
    }
}
