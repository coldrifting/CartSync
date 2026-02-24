using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Stores")]
[Route("/api/stores/[action]")]
public class StoreController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<StoreResponse> All()
    {
        return db.Stores
            .OrderBy(s => s.StoreName)
            .Select(s => s.ToResponse());
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public void Add([FromBody] StoreAddRenameRequest storeAddRenameRequest)
    {
        db.Add(new Store
        {
            StoreName = storeAddRenameRequest.StoreName
        });
        db.SaveChanges();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Edit([Required] Ulid storeId, [FromBody] StoreAddRenameRequest storeAddRenameRequest)
    {
        Store? s = db.Stores.FirstOrDefault(s => s.StoreId == storeId);
        if (s == null)
        {
            return NotFound("Store not found");
        }
        
        s.StoreName = storeAddRenameRequest.StoreName;
        db.SaveChanges();

        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete([Required] Ulid storeId)
    {
        Store? s = db.Stores.FirstOrDefault(s => s.StoreId == storeId);
        if (s == null)
        {
            return NotFound("Store not found");
        }

        db.Stores.Remove(s);
        db.SaveChanges();
        
        return NoContent();
    }
}