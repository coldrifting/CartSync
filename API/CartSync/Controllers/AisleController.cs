using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Models;
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
    public async Task<Results<Ok<List<AisleResponse>>, BadRequest<Error>, NotFound<Error>>> All([Required] Ulid storeId)
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
            .Select(Aisle.ToResponse)
            .ToList();
        
        return TypedResults.Ok(aisles);
    }

    [HttpPost]
    [Route("/api/aisles/add")]
    public async Task<Results<Created<AisleResponse>, BadRequest<Error>, NotFound<Error>>> Add([Required] Ulid storeId, AisleAddRequest aisleAddRequest)
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
            AisleName = aisleAddRequest.Name,
            SortOrder = s.Aisles.Count
        };
        
        Db.Add(aisle);
        await Db.SaveChangesAsync();

        return TypedResults.Created($"/api/aisles/{aisle.AisleId}", aisle.ToNewResponse);
    }

    [HttpPatch]
    [Route("/api/aisles/{aisleId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid aisleId, JsonPatchDocument<AisleEditRequest> aislePatch)
    {
        Aisle? aisle = await Db.Aisles
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
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/aisles/{aisleId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid aisleId)
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
        aisle.Store.Aisles.RefreshOrder();
        
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}