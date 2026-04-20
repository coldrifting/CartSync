using CartSync.Data.Entities;
using CartSync.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers.Core;

[ApiController]
[Authorize(AuthenticationSchemes = $"{CookieAuthenticationDefaults.AuthenticationScheme},{JwtBearerDefaults.AuthenticationScheme}")]
public class ControllerCore(CartSyncContext context) : ControllerBase
{
    protected CartSyncContext Db { get; } = context;

    private async Task<Ulid> GetUserId()
    {
        string? username = User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        return (await Db.Users.FirstAsync(u => u.Username == username)).UserId;
    }

    protected async Task<UserInfo?> GetUserInfo()
    {
        Ulid id = await GetUserId();
        return await Db.UserInfo.FindAsync(id);
    }
    
    protected async Task<Ulid> GetSelectedStoreId()
    {
        Ulid userId = await GetUserId();
        UserInfo userInfo = (await Db.UserInfo.FindAsync(userId))!;
        return userInfo.StoreId;
    }
    
    protected async Task<Store> GetSelectedStore()
    {
        Ulid storeId = await GetSelectedStoreId();
        Store store = (await Db.Stores.FindAsync(storeId))!;
        return store;
    }
    
    protected async Task SetSelectedStore(Ulid storeId)
    {
        Ulid userId = await GetUserId();
        UserInfo? userInfo = await Db.UserInfo.FindAsync(userId);
        if (userInfo == null)
        {
            Db.Add(new UserInfo
            {
                UserId = userId,
                StoreId = storeId
            });
        }
        else
        {
            userInfo.StoreId = storeId;
        }
        
        await Db.SaveChangesAsync();
    }
}