using System.Security.Claims;
using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Utils.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[AllowAnonymous]
[Tags("Users")]
public class UserController(CartSyncContext context, JwtAuthentication auth) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/user/login/token")]
    public async Task<Results<Ok<UserLoginSuccessResponse>, BadRequest<ErrorResponse>>> LoginToken(UserLoginRequest payload)
    {
        User? user = await Db.Users.FirstOrDefaultAsync(u => u.Username == payload.Username);

        if (user is null || !JwtAuthentication.IsPasswordValid(payload.Password, user.Hash, user.Salt))
        {
            return ErrorResponse.BadRequestInvalidLogin();
        }

        // Send JWT token to avoid expensive hash calls for each authenticated endpoint
        string token = auth.GenerateToken(user);
        return TypedResults.Ok(new UserLoginSuccessResponse(token));
    }
    
    [HttpPost]
    [Route("/api/user/login/cookie")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>>> LoginCookie(UserLoginRequest payload)
    {
        User? user = await Db.Users.FirstOrDefaultAsync(u => u.Username == payload.Username);

        if (user is null || !JwtAuthentication.IsPasswordValid(payload.Password, user.Hash, user.Salt))
        {
            return ErrorResponse.BadRequestInvalidLogin();
        }

        await HttpContext.SignInAsync(new ClaimsPrincipal([
            new ClaimsIdentity([
                new Claim("sub", user.Username)
            ], CookieAuthenticationDefaults.AuthenticationScheme)
        ]));

        // Send JWT token to avoid expensive hash calls for each authenticated endpoint
        string token = auth.GenerateToken(user);
        return TypedResults.NoContent();
    }
    
    [HttpPost]
    [Route("/api/user/logout/cookie")]
    public async Task<NoContent> LogoutCookie()
    {
        await HttpContext.SignOutAsync();
        return TypedResults.NoContent();
    }
}