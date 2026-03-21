using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Locations")]
public class AisleController(CartSyncContext db) : ControllerCore
{
    [HttpGet]
    [Route("/api/stores/{storeId}/aisles")]
    public async Task<Results<Ok<List<AisleResponse>>, BadRequest<Error>, NotFound<Error>>> All(Ulid storeId)
    {
        Store? s = await db.Stores
            .Include(s => s.Aisles)
            .ThenInclude(a => a.Items)
            .FirstOrDefaultAsync(store => store.StoreId == storeId);
        if (s == null)
        {
            return Store.NotFound(storeId);
        }
        
        List<AisleResponse> aisles = s.Aisles
            .OrderBy(a => a.SortOrder)
            .AsQueryable()
            .Select(Aisle.ToResponse)
            .ToList();
        
        return TypedResults.Ok(aisles);
    }
    
    [HttpGet]
    [Route("/api/stores/{storeId}/aisles/{aisleId}/usages")]
    public async Task<Results<Ok<UsageResponse>, BadRequest<Error>, NotFound<Error>>> Usages(Ulid storeId, Ulid aisleId)
    {
        Aisle? aisle = await db.Aisles
            .Include(a => a.Items)
            .FirstOrDefaultAsync(a => a.AisleId == aisleId);
        if (aisle == null)
        {
            return Aisle.NotFound(aisleId);
        }

        if (aisle.StoreId != storeId)
        {
            return Aisle.NotFoundUnderStore(aisleId, storeId);
        }

        IOrderedEnumerable<Item> items = aisle.Items
            .OrderBy(i => i.ItemName)
            .ThenBy(i => i.ItemId);
        
        UsageResponse result = new();
        result.Update(items, i => i.ItemId, i => i.ItemName);
        
        return TypedResults.Ok(result);
    }

    [HttpPost]
    [Route("/api/stores/{storeId}/aisles/add")]
    public async Task<Results<Created<AisleResponse>, BadRequest<Error>, NotFound<Error>>> Add(Ulid storeId, AisleAddRequest aisleAddRequest)
    {
        Store? s = await db.Stores
            .Include(store => store.Aisles)
            .FirstOrDefaultAsync(store => store.StoreId == storeId);
        if (s == null)
        {
            return Store.NotFound(storeId);
        }

        Aisle aisle = new()
        {
            StoreId = s.StoreId,
            AisleName = aisleAddRequest.AisleName,
            SortOrder = s.Aisles.Count
        };
        
        await db.AddAsync(aisle);
        await db.SaveChangesAsync();

        return TypedResults.Created($"/api/stores/{storeId}/aisles/{aisle.AisleId}", aisle.ToNewResponse);
    }

    [HttpPatch]
    [Route("/api/stores/{storeId}/aisles/{aisleId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid storeId, Ulid aisleId, JsonPatchDocument<AisleEditRequest> aislePatch)
    {
        Aisle? aisle = await db.Aisles
            .Include(aisle => aisle.Store)
            .ThenInclude(store => store.Aisles)
            .FirstOrDefaultAsync(aisle => aisle.AisleId == aisleId);
        if (aisle == null)
        {
            return Aisle.NotFound(aisleId);
        }

        if (!TryGetEditObject(aisle, aislePatch, out AisleEditRequest? aisleEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }
        
        aisle.UpdateFromEditRequest(aisleEdit);
        await db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/stores/{storeId}/aisles/{aisleId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid storeId, Ulid aisleId)
    {
        Aisle? aisle = await db.Aisles
            .Include(aisle => aisle.Store)
            .ThenInclude(store => store.Aisles)
            .FirstOrDefaultAsync(aisle => aisle.AisleId == aisleId);
        if (aisle == null)
        {
            return Aisle.NotFound(aisleId);
        }

        if (aisle.StoreId != storeId)
        {
            return Aisle.NotFoundUnderStore(aisleId, storeId);
        }

        db.Aisles.Remove(aisle);
        
        // Refresh Sort Order
        aisle.Store.Aisles.RefreshOrder();
        
        await db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}