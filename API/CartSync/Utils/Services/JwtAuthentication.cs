using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CartSync.Models;
using JetBrains.Annotations;

namespace CartSync.Utils.Services;

public record TokenHeaderResponse(string Alg = "HS256", string Typ = "JWT");
public record TokenPayloadResponse(string Iss, string Sub, long Exp);

[UsedImplicitly]
public class JwtAuthentication(string secret)
{
	private string Secret { get; } = secret;

	// Expire time of generated JWT tokens in minutes
    private const int ExpireTime = 120; 

    public string GenerateToken(User user)
    {
	    long expireTime = DateTimeOffset.UtcNow.AddMinutes(ExpireTime).ToUnixTimeSeconds();
	    
        TokenHeaderResponse headerResponse = new();
        
        TokenPayloadResponse payloadResponse = new(
	        "CartSyncAPI", 
	        user.Username,
	        expireTime);

        JsonSerializerOptions serializeOptions = new()
        {
	        Converters = { new JsonStringEnumConverter() },
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        string header = JsonSerializer.Serialize(headerResponse, serializeOptions);
        string payload = JsonSerializer.Serialize(payloadResponse, serializeOptions);
        string signature = GenerateJwtSignature(header, payload);

        return Hash.Base64Encode(header) + '.' + 
               Hash.Base64Encode(payload) + '.' + 
               signature;
    }
    
    public bool IsPasswordValid(string password, byte[] hash, byte[] salt)
    {
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, Hash.Iterations, HashAlgorithmName.SHA512, Hash.KeySize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, hash);
    }
    
    // Generates a base64url encoded signature based on two strings
    private string GenerateJwtSignature(string header, string payload)
    {
        string encodedHeader = Hash.Base64Encode(header);
        string encodedPayload = Hash.Base64Encode(payload);

        byte[] key = Encoding.UTF8.GetBytes(Secret);
        byte[] source = Encoding.UTF8.GetBytes(encodedHeader + '.' + encodedPayload);

        return Hash.Base64EncodeBytes(HMACSHA256.HashData(key, source));
    }
}