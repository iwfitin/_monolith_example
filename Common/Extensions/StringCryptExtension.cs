using System.Security.Cryptography;
using System.Text;

namespace Common.Extensions;

public static class StringCryptExtension
{
    private static byte[] Key { get; }

    private static byte[] Iv { get; }

    static StringCryptExtension()
    {
        Key = "ac146f90682b40cb9eabb6aeccf9b0e5".ToBytes();
        Iv = "a6fe594e804a466583648606ca136fc5".ToBytes();
    }

    public static int BitsCountInByte => 8;

    public static byte[] ToBytes(this string str)
    {
        return Encoding.UTF8.GetBytes(str);
    }

    public static string AesEncrypt(this string str)
    {
        return str.AesEncrypt(Key, Iv);
    }

    public static string AesDecrypt(this string str)
    {
        return str.AesDecrypt(Key, Iv);
    }

    public static string AesEncrypt(this string str, byte[] keySource, byte[] ivSource)
    {
        return str.Encrypt(Aes.Create(), keySource, ivSource);
    }

    public static string AesDecrypt(this string str, byte[] keySource, byte[] ivSource)
    {
        return str.Decrypt(Aes.Create(), keySource, ivSource);
    }

    public static string Encrypt(this string str, SymmetricAlgorithm algorithm, byte[] keySource, byte[] ivSource)
    {
        var (key, iv) = SliceKeys(algorithm, keySource, ivSource);
        return str.SymmetricTransform(algorithm.CreateEncryptor, Encoding.UTF8.GetBytes, Convert.ToBase64String, key, iv);
    }

    public static string Decrypt(this string str, SymmetricAlgorithm algorithm, byte[] keySource, byte[] ivSource)
    {
        var (key, iv) = SliceKeys(algorithm, keySource, ivSource);
        return str.SymmetricTransform(algorithm.CreateDecryptor, Convert.FromBase64String, Encoding.UTF8.GetString, key, iv);
    }

    private static (byte[] Key, byte[] Iv) SliceKeys(SymmetricAlgorithm algorithm, byte[] keySource, byte[] ivSource)
    {
        var keyMinSize = algorithm.LegalKeySizes.FirstOrDefault()?.MinSize / BitsCountInByte
                         ?? throw new ArgumentException($"2566. legalKeySizes = {algorithm.LegalBlockSizes}");

        var ivMinSize = algorithm.LegalBlockSizes.FirstOrDefault()?.MinSize / BitsCountInByte
                        ?? throw new ArgumentException($"2546. legalBlockSizes = {algorithm.LegalBlockSizes}");

        if (keySource.Length < keyMinSize)
            throw new ArgumentException($"2543. the length of the key is not sufficient, required length = {keyMinSize}.");

        if (ivSource.Length < ivMinSize)
            throw new ArgumentException($"2542. the length of the block is not sufficient, required length = {ivMinSize}.");

        return (keySource.Take(keyMinSize).ToArray(), ivSource.Take(ivMinSize).ToArray());
    }

    private static string SymmetricTransform(this string str, Func<byte[], byte[], ICryptoTransform> createTransformer,
        Func<string, byte[]> strToBytes, Func<byte[], string> bytesToStr, byte[] key, byte[] iv)
    {
        var @in = strToBytes(str);
        var @out = createTransformer(key, iv).TransformFinalBlock(@in, 0, @in.Length);

        return bytesToStr(@out);
    }

    public static string HmacSha256Hex(this byte[] msg, byte[] key)
    {
        return BitConverter.ToString(msg.HmacSha256(key)).Replace("-", string.Empty);
    }

    public static string HmacSha256Base64(this byte[] msg, byte[] key)
    {
        return Convert.ToBase64String(msg.HmacSha256(key));
    }

    public static byte[] HmacSha256(this byte[] msg, byte[] key)
    {
        return new HMACSHA256(key).ComputeHash(msg);
    }

    public static string Sha256(this string payload)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(payload.ToBytes()));
    }
}
