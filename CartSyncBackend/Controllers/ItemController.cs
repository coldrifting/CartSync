using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Items")]
[Route("/api/items/[action]")]
public class ItemController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult All(Ulid? storeId)
    {
        if (storeId == null)
        {
            return Ok(db.Items.Select(i => i.ToResponse()));
        }

        Store? s = db.Stores.Find(storeId.Value);
        if (s == null)
        {
            return NotFound("Store not found");
        }

        var x = db.ItemAisles
            .Include(a => a.Store)
            .Include(a => a.Aisle)
            .Include(a => a.Item)
            .Where(a => a.StoreId == s.StoreId)
            .Select(ia => new
            {
                AisleId = ia.AisleId,
                AisleName = ia.Aisle.AisleName,
                ItemId = ia.ItemId,
                ItemName = ia.Item.ItemName
            });
        
        return Ok(x);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Add([FromBody] ItemAddRequest itemAddRequest)
    {
        db.Add(new Item
        {
            ItemName = itemAddRequest.ItemName,
            ItemTemp = itemAddRequest.ItemTemp ?? ItemTemp.Ambient,
            DefaultUnitType = itemAddRequest.DefaultUnitType ?? UnitType.Count
        });
        db.SaveChanges();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Edit([Required] Ulid itemId, [FromBody] ItemEditRequest itemEditRequest)
    {
        Item? i = db.Items.FirstOrDefault(i => i.ItemId == itemId);
        if (i == null)
        {
            return NotFound("Item not found");
        }
        
        i.ItemName = itemEditRequest.ItemName ?? i.ItemName;
        i.ItemTemp = itemEditRequest.ItemTemp ?? i.ItemTemp;
        i.DefaultUnitType = itemEditRequest.DefaultUnitType ?? i.DefaultUnitType;
        db.SaveChanges();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Location([Required] Ulid itemId, [Required] Ulid storeId, [FromBody] ItemAisleLocChangeRequest itemAisleLocChangeRequest)
    {
        Item? i = db.Items.FirstOrDefault(i => i.ItemId == itemId);
        if (i == null)
        {
            return NotFound("Item not found");
        }
        
        Store? s = db.Stores
            .Include(s => s.Aisles)
            .FirstOrDefault(s => s.StoreId == storeId);
        if (s == null)
        {
            return NotFound("Store not found");
        }

        if (s.Aisles.All(a => a.AisleId != itemAisleLocChangeRequest.AisleId))
        {
            return NotFound("Aisle not found");
        }
        
        ItemAisle? itemAisle = db.ItemAisles.FirstOrDefault(ia => ia.ItemId == itemId && ia.StoreId == s.StoreId);
        if (itemAisle == null)
        {
            ItemAisle newItemAisle = new()
            {
                AisleId = itemAisleLocChangeRequest.AisleId,
                ItemId = itemId,
                Bay = itemAisleLocChangeRequest.Bay ?? BayType.Middle
            };
            db.ItemAisles.Add(newItemAisle);
        }
        else
        {
            itemAisle.AisleId = itemAisleLocChangeRequest.AisleId;
            itemAisle.Bay = itemAisleLocChangeRequest.Bay ?? BayType.Middle;
        }

        db.SaveChanges();
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(Ulid itemId)
    {
        Item? i = db.Items.FirstOrDefault(i => i.ItemId == itemId);
        if (i == null)
        {
            return NotFound("Item not found");
        }

        db.Items.Remove(i);
        db.SaveChanges();
        
        return NoContent();
    }
}