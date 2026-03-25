using CartSync.Controllers.Core;
using CartSync.Models;
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
    public async Task<Results<Ok<UserLoginSuccessResponse>, BadRequest<Error>>> Login(UserLoginRequest payload)
    {
        User? user = await Db.Users.FirstOrDefaultAsync(u => u.Username == payload.Username);

        if (user is null || !JwtAuthentication.IsPasswordValid(payload.Password, user.Hash, user.Salt))
        {
            return Error.BadRequestInvalidLogin();
        }

        // Send JWT token to avoid expensive hash calls for each authenticated endpoint
        string token = auth.GenerateToken(user);
        return TypedResults.Ok(new UserLoginSuccessResponse(token));
    }
}