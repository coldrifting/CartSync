using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Locations")]
public class AisleController(CartSyncContext db) : ControllerCore
{
    [HttpGet]
    [Route("/api/stores/{storeId}/aisles")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AisleResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> All(Ulid storeId)
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
        
        return Ok(aisles);
    }
    
    [HttpGet]
    [Route("/api/stores/{storeId}/aisles/{aisleId}/usages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsageResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Usages(Ulid storeId, Ulid aisleId)
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
        
        return Ok(result);
    }

    [HttpPost]
    [Route("/api/stores/{storeId}/aisles/add")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AisleResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Add(Ulid storeId, AisleAddRequest aisleAddRequest)
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

        return Created($"/api/stores/{storeId}/aisles/{aisle.AisleId}", aisle.ToNewResponse);
    }

    [HttpPatch]
    [Route("/api/stores/{storeId}/aisles/{aisleId}/edit")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid storeId, Ulid aisleId, JsonPatchDocument<AisleEditRequest> aislePatch)
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
        
        return NoContent();
    }
    
    [HttpDelete]
    [Route("/api/stores/{storeId}/aisles/{aisleId}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid storeId, Ulid aisleId)
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
        
        return NoContent();
    }
}