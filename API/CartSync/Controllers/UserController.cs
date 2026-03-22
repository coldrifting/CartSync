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
[Route("/api/user/[action]")]
public class UserController(CartSyncContext db, JwtAuthentication auth) : ControllerCore
{
    [HttpPost]
    public async Task<Results<Ok<UserLoginSuccessResponse>, BadRequest<Error>>> Login(UserLoginRequest payload)
    {
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Username == payload.Username);

        if (user is null || !auth.IsPasswordValid(payload.Password, user.Hash, user.Salt))
        {
            return Error.BadRequestInvalidLogin();
        }

        // Send JWT token to avoid expensive hash calls for each authenticated endpoint
        string token = auth.GenerateToken(user);
        return TypedResults.Ok(new UserLoginSuccessResponse(token));
    }
}