using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Objects;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Locations")]
public class AisleController(CartSyncContext context) : ControllerCore(context)
{
    [HttpGet]
    [Route("/api/aisles")]
    public async Task<Results<Ok<ReadOnlyList<AisleResponse>>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> All(Ulid? storeId)
    {
        if (storeId is null)
        {
            ReadOnlyList<AisleResponse> allAisles = Db.Aisles
                .Select(AisleResponse.FromEntity)
                .OrderBy(aisle => aisle.SortOrder)
                .ToReadOnlyList();
            
            return TypedResults.Ok(allAisles);
        }
        
        if (await Db.Stores.FindAsync(storeId) is null)
        {
            return Store.NotFound(storeId.Value);
        }

        ReadOnlyList<AisleResponse> aisles = Db.Stores
            .AsNoTracking()
            .Include(store => store.Aisles)
            .First(store => store.StoreId == storeId)
            .Aisles
            .Select(AisleResponse.FromEntity)
            .OrderBy(aisle => aisle.SortOrder)
            .ToReadOnlyList();
        
        return TypedResults.Ok(aisles);
    }
    
    [HttpGet]
    [Route("/api/aisles/selected")]
    public async Task<Ok<ReadOnlyList<AisleResponse>>> AllSelected()
    {
        Ulid storeId = await GetSelectedStoreId();
        
        ReadOnlyList<AisleResponse> aisles = Db.Aisles
            .AsNoTracking()
            .Where(aisle => aisle.StoreId == storeId)
            .OrderBy(aisle => aisle.SortOrder)
            .Select(AisleResponse.FromEntity)
            .ToReadOnlyList();
        
        return TypedResults.Ok(aisles);
    }

    [HttpPost]
    [Route("/api/aisles/add")]
    public async Task<Results<Created<AisleResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Add([Required] Ulid storeId, AddRequest addRequest)
    {
        Store? s = await Db.Stores
            .Include(store => store.Aisles)
            .FirstOrDefaultAsync(store => store.StoreId == storeId);
        if (s == null)
        {
            return Store.NotFound(storeId);
        }

        Aisle aisle = new()
        {
            StoreId = s.StoreId,
            AisleName = addRequest.Name,
            SortOrder = s.Aisles.Count
        };
        
        Db.Add(aisle);
        await Db.SaveChangesAsync();

        return TypedResults.Created($"/api/aisles/{aisle.AisleId}", AisleResponse.FromNewEntity(aisle));
    }

    [HttpPatch]
    [Route("/api/aisles/{aisleId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Edit(Ulid aisleId, JsonPatchDocument<AisleEditRequest> jsonPatch)
    {
        Aisle? aisle = await Db.Aisles
            .Include(aisle => aisle.Store)
            .ThenInclude(store => store.Aisles)
            .FirstOrDefaultAsync(aisle => aisle.AisleId == aisleId);
        if (aisle == null)
        {
            return Aisle.NotFound(aisleId);
        }
        
        if (!aisle.TryGetPatch(ModelState, jsonPatch, out AisleEditRequest patch))
        {
            return ErrorResponse.BadRequestPatchInvalid(ModelState);
        }

        aisle.ApplyPatch(patch);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/aisles/{aisleId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Delete(Ulid aisleId)
    {
        if (await Db.Aisles.FindAsync(aisleId) is not { } aisle)
        {
            return Aisle.NotFound(aisleId);
        }

        Db.Aisles.Remove(aisle);

        Store store = await Db.Stores
            .Include(store => store.Aisles)
            .FirstAsync(store => store.StoreId == aisle.StoreId);
        
        // Refresh Sort Order
        Sort.RefreshOrder(store.Aisles);
        
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}