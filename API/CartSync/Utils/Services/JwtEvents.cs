using System.Security.Claims;
using CartSync.Controllers.Core;
using CartSync.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace CartSync.Utils.Services;

public static class JwtEvents
{
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
}