using System.Security.Cryptography;
using System.Text;

namespace DxRating.Common.Utils;

public static class RandomUtils
{
    private static ReadOnlySpan<char> Alphabet => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static ReadOnlySpan<char> AlphaNumeric => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GetRandomAlphabetString(int length)
    {
        return RandomNumberGenerator.GetString(Alphabet, length);
    }

    public static string GetRandomAlphaNumericString(int length)
    {
        return RandomNumberGenerator.GetString(AlphaNumeric, length);
    }

    public static byte[] GetRandomBytes(int length)
    {
        return RandomNumberGenerator.GetBytes(length);
    }

    public static string GetRandomByteString(int length)
    {
        var bytes = GetRandomBytes(length);
        return Encoding.UTF8.GetString(bytes);
    }

    public static string GetRandomBase64String(int byteLenght)
    {
        var bytes = new byte[byteLenght];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
