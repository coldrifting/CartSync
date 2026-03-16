using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Stores - Aisles")]
[Route("/api/aisles/[action]")]
public class AisleController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AisleResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> All(Ulid storeId)
    {
        if (!ModelState.IsValid)
        {
            return Error.BadRequestInvalidStoreId;
        }
        
        Store? s = await db.Stores
            .Include(s => s.Aisles)
            .GetAsync(storeId);
        if (s == null)
        {
            return Error.BadRequestInvalidStoreId;
        }

        List<AisleResponse> aisles = s.Aisles
            .OrderBy(a => a.SortOrder)
            .Select(a => new AisleResponse
            {
                AisleId = a.AisleId,
                AisleName = a.AisleName,
                SortOrder = a.SortOrder
            })
            .ToList();
        
        return Ok(aisles);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Add([FromBody] AisleAddRequest aisleAddRequest)
    {
        Store? s = await db.Stores
            .Include(store => store.Aisles)
            .GetAsync(aisleAddRequest.StoreId);
        if (s == null)
        {
            return Error.BadRequestInvalidStoreId;
        }
        
        db.Add(new Aisle
        {
            StoreId = s.StoreId,
            AisleName = aisleAddRequest.AisleName,
            SortOrder = s.Aisles.Count
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid aisleId, [FromBody] AisleEditRequest aisleEditRequest)
    {
        Aisle? a = await db.Aisles
            .Include(aisle => aisle.Store)
            .ThenInclude(store => store.Aisles)
            .GetAsync(aisleId);
        if (a == null)
        {
            return Error.NotFoundAisle;
        }
        
        a.AisleName = aisleEditRequest.AisleName ?? a.AisleName;
        
        if (aisleEditRequest.SortOrder is { } newIndex)
        {
            int oldIndex = a.SortOrder;
            a.Store.Aisles.Reorder(oldIndex, newIndex);
        }
        
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid aisleId)
    {
        Aisle? a = await db.Aisles
            .Include(aisle => aisle.Store)
            .ThenInclude(store => store.Aisles)
            .GetAsync(aisleId);
        if (a == null)
        {
            return Error.NotFoundAisle;
        }

        db.Aisles.Remove(a);
        
        // Refresh Sort Order
        a.Store.Aisles.RefreshOrder();
        
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}