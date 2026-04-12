using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;

namespace CartSync.Controllers;

[Tags("Locations")]
public class StoreController(CartSyncContext context) : ControllerCore(context)
{
    [HttpGet]
    [Route("/api/stores")]
    public async Task<Ok<ReadOnlyList<StoreResponse>>> All()
    {
        Ulid selectedStoreId = await GetSelectedStoreId();
        ReadOnlyList<StoreResponse> stores = await Db.Stores
            .OrderBy(store => store.StoreName)
            .Select(StoreResponse.FromEntity(selectedStoreId))
            .ToReadOnlyListAsync();

        return TypedResults.Ok(stores);
    }
    
    [HttpPost]
    [Route("/api/stores/add")]
    public async Task<Results<Created<StoreResponse>, BadRequest<ErrorResponse>>> Add(AddRequest addRequest)
    {
        Store store = new()
        {
            StoreName = addRequest.Name,
        };
        
        Db.Add(store);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/stores/{store.StoreId}", StoreResponse.FromNewEntity(store));
    }

    [HttpPut]
    [Route("/api/stores/{storeId}/select")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Select(Ulid storeId)
    {
        if (await Db.Stores.FindAsync(storeId) is null)
        {
            return Store.NotFound(storeId);
        }
        
        await SetSelectedStore(storeId);
        return TypedResults.NoContent();
    }

    [HttpPatch]
    [Route("/api/stores/{storeId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Edit(Ulid storeId, JsonPatchDocument<StoreEditRequest> jsonPatch)
    {
        if (await Db.Stores.FindAsync(storeId) is not { } store)
        {
            return Store.NotFound(storeId);
        }

        if (!store.TryGetPatch(ModelState, jsonPatch, out StoreEditRequest patch))
        {
            return ErrorResponse.BadRequestPatchInvalid(ModelState);
        }

        store.ApplyPatch(patch);
        await Db.SaveChangesAsync();

        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/stores/{storeId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>, Conflict<ErrorResponse>>> Delete(Ulid storeId)
    {
        if (await Db.Stores.FindAsync(storeId) is not { } store)
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