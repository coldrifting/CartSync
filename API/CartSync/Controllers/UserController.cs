using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Utils.Services;
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
    [Route("/api/user/login")]
    public async Task<Results<Ok<UserLoginSuccessResponse>, BadRequest<ErrorResponse>>> Login(UserLoginRequest payload)
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
}