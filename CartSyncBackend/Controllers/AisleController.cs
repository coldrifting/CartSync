using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Aisles")]
[Route("/api/aisles/[action]")]
public class AisleController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult All(Ulid storeId)
    {
        Store? s = db.Stores
            .Include(s => s.Aisles)
            .FirstOrDefault(s => s.StoreId == storeId);
        if (s == null)
        {
            return Error.NotFoundStore;
        }

        List<AisleResponse> aisles = s.Aisles
            .OrderBy(a => a.AisleOrder)
            .Select(a => new AisleResponse()
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
    public IActionResult Add([FromBody] AisleAddRequest aisleAddRequest)
    {
        Store? s = db.Stores.FirstOrDefault(s => s.StoreId == aisleAddRequest.StoreId);
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
        db.SaveChanges();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Rename(Ulid aisleId, [FromBody] AisleRenameRequest aisleRenameRequest)
    {
        Aisle? a = db.Aisles.FirstOrDefault(s => s.AisleId == aisleId);
        if (a == null)
        {
            return Error.NotFoundAisle;
        }
        
        a.AisleName = aisleRenameRequest.AisleName;
        db.SaveChanges();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Reorder(Ulid aisleId, [FromBody] AisleReorderRequest aisleReorderRequest)
    {
        Aisle? a = db.Aisles.FirstOrDefault(s => s.AisleId == aisleId);
        if (a == null)
        {
            return Error.NotFoundAisle;
        }
        
        a.AisleOrder = aisleReorderRequest.AisleOrder;
        db.SaveChanges();
        
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Ulid aisleId)
    {
        Aisle? a = db.Aisles.FirstOrDefault(a => a.AisleId == aisleId);
        if (a == null)
        {
            return Error.NotFoundAisle;
        }

        db.Aisles.Remove(a);
        db.SaveChanges();
        
        return NoContent();
    }
}