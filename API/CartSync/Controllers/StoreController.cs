using CartSync.Controllers.Core;
using CartSync.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Locations")]
public class StoreController(CartSyncContext db) : ControllerCore
{
    [HttpGet]
    [Route("/api/stores")]
    public async Task<Ok<List<StoreResponse>>> All()
    {
        List<StoreResponse> stores = await db.Stores
            .OrderBy(s => s.StoreName)
            .Select(Store.ToResponse)
            .ToListAsync();

        return TypedResults.Ok(stores);
    }
    
    [HttpPost]
    [Route("/api/stores/add")]
    public async Task<Results<Created<StoreResponse>, BadRequest<Error>>> Add([FromBody] StoreAddRequest storeAddRequest)
    {
        Store store = new()
        {
            StoreName = storeAddRequest.StoreName,
        };
        
        await db.AddAsync(store);
        await db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/stores/{store.StoreId}", store.ToNewResponse);
    }

    [HttpPatch]
    [Route("/api/stores/{storeId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid storeId, [FromBody] JsonPatchDocument<StoreEditRequest> storePatch)
    {
        Store? store = await db.Stores.FindAsync(storeId);
        if (store == null)
        {
            return Store.NotFound(storeId);
        }

        if (!TryGetEditObject(store, storePatch, out StoreEditRequest? storeEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        store.UpdateFromEditRequest(storeEdit);
        await db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/stores/{storeId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid storeId)
    {
        Store? store = await db.Stores.FindAsync(storeId);
        if (store == null)
        {
            return Store.NotFound(storeId);
        }

        db.Stores.Remove(store);
        await db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}