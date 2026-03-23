using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using CartSync.Models;
using CartSync.Models.Interfaces;
using CartSync.Models.Joins;
using CartSync.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers.Core;

[ApiController]
[Authorize]
public class ControllerCore(CartSyncContext context) : ControllerBase
{
    protected CartSyncContext Db { get; } = context;

    private async Task<Ulid> GetUserId()
    {
        string? username = User.Username;
        return (await Db.Users.FirstAsync(u => u.Username == username)).UserId;
    }
    
    protected async Task<Ulid> GetSelectedStoreId()
    {
        Ulid userId = await GetUserId();
        SelectedStore selStore = (await Db.SelectedStores.FindAsync(userId))!;
        return selStore.StoreId;
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
        SelectedStore? selStore = await Db.SelectedStores.FindAsync(userId);
        if (selStore == null)
        {
            Db.Add(new SelectedStore
            {
                UserId = userId,
                StoreId = storeId
            });
        }
        else
        {
            selStore.StoreId = storeId;
        }
        
        await Db.SaveChangesAsync();
    }
        
    protected bool TryGetEditObject<TEdit>(IEditable<TEdit> source, JsonPatchDocument<TEdit> patch, [NotNullWhen(true)] out TEdit? editRequest, Ulid? storeId = null)
        where TEdit : class
    {
        editRequest = source.ToEditRequest(storeId);

        patch.ApplyTo(editRequest, jsonPatchError =>
        {
            string key = jsonPatchError.AffectedObject.GetType().Name;
            ModelState.AddModelError(key, jsonPatchError.ErrorMessage);
        });

        bool isModelValid = TryValidateModel(editRequest);
        
        return ModelState.IsValid && isModelValid;
    }
}