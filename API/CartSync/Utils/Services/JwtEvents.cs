using CartSync.Data.Responses;
using CartSync.Database;
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

	            ErrorResponse error = ErrorResponse.Unauthorized();
	            return context.Response.WriteAsJsonAsync(error);
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

			    string? username = context.Principal?.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
			    if (dbContext.Users.Any(u => u.Username == username))
			    {
				    return Task.CompletedTask;
			    }

			    context.Fail("Invalid Token");
				return Task.CompletedTask;
		    }
	    };
    }
}