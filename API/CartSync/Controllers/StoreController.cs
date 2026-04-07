using CartSync.Controllers.Core;
using CartSync.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Locations")]
public class StoreController(CartSyncContext context) : ControllerCore(context)
{
    [HttpGet]
    [Route("/api/stores")]
    public async Task<Ok<List<StoreResponse>>> All()
    {
        Ulid selectedStoreId = await GetSelectedStoreId();
        List<StoreResponse> stores = await Db.Stores
            .OrderBy(s => s.StoreName)
            .Select(Store.ToResponse)
            .ToListAsync();

        for (int index = 0; index < stores.Count; index++)
        {
            StoreResponse storeResponse = stores[index];
            if (storeResponse.Id == selectedStoreId)
            {
                stores[index] = storeResponse with { IsSelected = true };
            }
        }

        return TypedResults.Ok(stores);
    }
    
    [HttpPost]
    [Route("/api/stores/add")]
    public async Task<Results<Created<StoreResponse>, BadRequest<Error>>> Add([FromBody] StoreAddRequest storeAddRequest)
    {
        Store store = new()
        {
            StoreName = storeAddRequest.Name,
        };
        
        Db.Add(store);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/stores/{store.StoreId}", store.ToNewResponse);
    }

    [HttpPost]
    [Route("/api/stores/{storeId}/select")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Select(Ulid storeId, [FromBody] object? x = null)
    {
        Store? store = await Db.Stores.FindAsync(storeId);
        if (store == null)
        {
            return Store.NotFound(storeId);
        }
        
        await SetSelectedStore(storeId);
        return TypedResults.NoContent();
    }

    [HttpPatch]
    [Route("/api/stores/{storeId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid storeId, [FromBody] JsonPatchDocument<StoreEditRequest> storePatch)
    {
        Store? store = await Db.Stores.FindAsync(storeId);
        if (store == null)
        {
            return Store.NotFound(storeId);
        }

        if (!TryGetEditObject(store, storePatch, out StoreEditRequest? storeEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        store.UpdateFromEditRequest(storeEdit);
        await Db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/stores/{storeId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>, Conflict<Error>>> Delete(Ulid storeId)
    {
        Store? store = await Db.Stores.FindAsync(storeId);
        if (store == null)
        {
            return Store.NotFound(storeId);
        }

        Store selectedStore = await GetSelectedStore();
        if (selectedStore == store)
        {
            return Store.ConflictSelected(storeId);
        }

        Db.Stores.Remove(store);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}