using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CartSync.Utils.Services;

public record TokenHeaderResponse(string Alg = "HS256", string Typ = "JWT");
public record TokenPayloadResponse(string Iss, string Sub, long Exp);

[UsedImplicitly]
public class Authorization
{
    // Generates a new secret JWT signing key each time the server is run
    // This simplifies things, and means that no secret keys will be accidentally uploaded,
    // but it also means that any signed-in users will need to re-sign-in if the server is reset
    private static readonly string Secret = Hash.GenerateHash();
    public static byte[] SecretBytes => Encoding.UTF8.GetBytes(Secret);
    
    // Expire time of generated JWT tokens in minutes
    private const int ExpireTime = 120; 

    public static string GenerateToken(User user)
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
    
    // Generates a base64url encoded signature based on two strings
    private static string GenerateJwtSignature(string header, string payload)
    {
        string encodedHeader = Hash.Base64Encode(header);
        string encodedPayload = Hash.Base64Encode(payload);

        byte[] key = Encoding.UTF8.GetBytes(Secret);
        byte[] source = Encoding.UTF8.GetBytes(encodedHeader + '.' + encodedPayload);

        return Hash.Base64EncodeBytes(HMACSHA256.HashData(key, source));
    }
    
    // Customize error response body on 401 Unauthorized and 403 Forbidden
    public static JwtBearerEvents BearerEvents()
    {
	    return new JwtBearerEvents
	    {
		    OnChallenge = context =>
		    {
			    context.HandleResponse();
			    
	            context.Response.ContentType = "application/json";
	            context.Response.StatusCode = 401;

	            return context.Response.WriteAsJsonAsync(Error.Unauthorized);
		    },
		    OnAuthenticationFailed = context =>
		    {
			    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
			    {
					context.Response.Headers.Append("Token-Expired", "true");
			    }
			    
			    return Task.CompletedTask;
		    },
		    OnTokenValidated = context =>
		    {
			    CartSyncContext dbContext = context.HttpContext.RequestServices.GetRequiredService<CartSyncContext>();

			    if (context.Principal?.Claims is {} claims)
			    {
				    string? username = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
				    if (dbContext.Users.Any(u => u.Username == username))
				    {
					    return Task.CompletedTask;
				    }
			    }

			    context.Fail("Invalid Token");
				return Task.CompletedTask;
		    }
	    };
    }
    
    //
    // Password generation / verification
    //
    
    public static bool VerifyPassword(string password, byte[] hash, byte[] salt)
    {
        byte[] hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, KeySize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, hash);
    }

    private const int KeySize = 64;
    private const int Iterations = 350000;
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