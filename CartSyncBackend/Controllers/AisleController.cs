using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Stores - Aisles")]
[Route("/api/aisles/[action]")]
public class AisleController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> All(Ulid storeId)
    {
        Store? s = await db.Stores
            .Include(s => s.Aisles)
            .FirstOrDefaultAsync(s => s.StoreId == storeId);
        if (s == null)
        {
            return Error.NotFoundStore;
        }

        List<AisleResponse> aisles = s.Aisles
            .OrderBy(a => a.AisleOrder)
            .Select(a => new AisleResponse
            {
                AisleId = a.AisleId,
                AisleName = a.AisleName,
                AisleOrder = a.AisleOrder
            })
            .ToList();
        
        return Ok(aisles);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add([FromBody] AisleAddRequest aisleAddRequest)
    {
        Store? s = await db.Stores.FirstOrDefaultAsync(s => s.StoreId == aisleAddRequest.StoreId);
        if (s == null)
        {
            return Error.NotFoundStore;
        }

        db.Add(new Aisle
        {
            StoreId = s.StoreId,
            AisleName = aisleAddRequest.AisleName,
            AisleOrder = 0
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(Ulid aisleId, [FromBody] AisleRenameRequest aisleRenameRequest)
    {
        Aisle? a = await db.Aisles.FirstOrDefaultAsync(s => s.AisleId == aisleId);
        if (a == null)
        {
            return Error.NotFoundAisle;
        }
        
        a.AisleName = aisleRenameRequest.AisleName;
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reorder(Ulid aisleId, [FromBody] AisleReorderRequest aisleReorderRequest)
    {
        Aisle? a = await db.Aisles.FirstOrDefaultAsync(s => s.AisleId == aisleId);
        if (a == null)
        {
            return Error.NotFoundAisle;
        }
        
        a.AisleOrder = aisleReorderRequest.AisleOrder;
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Ulid aisleId)
    {
        Aisle? a = await db.Aisles.FirstOrDefaultAsync(a => a.AisleId == aisleId);
        if (a == null)
        {
            return Error.NotFoundAisle;
        }

        db.Aisles.Remove(a);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}