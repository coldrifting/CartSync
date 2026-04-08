using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
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
    public async Task<Results<Ok<List<AisleResponse>>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> All([Required] Ulid storeId)
    {
        Store? s = await Db.Stores
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
            .Select(AisleResponse.FromEntity)
            .ToList();
        
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
        Aisle? aisle = await Db.Aisles
            .Include(aisle => aisle.Store)
            .ThenInclude(store => store.Aisles)
            .FirstOrDefaultAsync(aisle => aisle.AisleId == aisleId);
        if (aisle == null)
        {
            return Aisle.NotFound(aisleId);
        }

        Db.Aisles.Remove(aisle);
        
        // Refresh Sort Order
        Sort.RefreshOrder(aisle.Store.Aisles);
        
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}