using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Aisles")]
[Route("/api/stores/aisles/[action]")]
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
            return NotFound("Store not found");
        }
        
        return Ok(s.Aisles.Select(a => a.ToResponse()));
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
            return NotFound("Store not found");
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
            return NotFound("Aisle not found");
        }
        
        a.AisleName = aisleRenameRequest.AisleName;
        db.SaveChanges();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Reorder(string aisleId, [FromBody] AisleReorderRequest aisleReorderRequest)
    {
        if (!Ulid.TryParse(aisleId, out Ulid aisleUlid))
        {
            return BadRequest("Invalid Aisle Id");
        }
        
        Aisle? a = db.Aisles.FirstOrDefault(s => s.AisleId == aisleUlid);
        if (a == null)
        {
            return NotFound("Aisle not found");
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
            return NotFound("Aisle not found");
        }

        db.Aisles.Remove(a);
        db.SaveChanges();
        
        return NoContent();
    }
}