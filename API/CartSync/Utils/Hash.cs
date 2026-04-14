using System.Security.Cryptography;
using System.Text;

namespace CartSync.Utils;

public static class Hash
{
    public static string GenerateHash()
    {
        return Convert.ToBase64String(new HMACSHA256().Key);
    }

    public static string Base64Encode(string input)
    {
        return Base64EncodeBytes(Encoding.UTF8.GetBytes(input));
    }

    public static string Base64EncodeBytes(byte[] bytes)
    {
        string base64 = Convert.ToBase64String(bytes);
        return base64.Replace('+', '-')
            .Replace('/', '_')
            .Replace("=", "");
    }

    public const int KeySize = 64;
    public const int Iterations = 350000;
    public static byte[] HashPassword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(KeySize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithmName.SHA512,
            KeySize);
         return hash;
    }
}