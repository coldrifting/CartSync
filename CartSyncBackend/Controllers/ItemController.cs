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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult All(Ulid? storeId)
    {
        if (storeId == null)
        {
            return Ok(db.Items.Select(i => i.ToResponse()));
        }

        Store? s = db.Stores.Find(storeId.Value);
        if (s == null)
        {
            return Error.NotFoundStore;
        }

        var items = db.ItemAisles
            .Where(i => i.StoreId == s.StoreId)
            .Select(ia => ia.Item);
        
        var itemAisles = db.ItemAisles
            .Where(ia => ia.StoreId == s.StoreId)
            .GroupBy(ia => ia.AisleId)
            .Select(g => new ItemAisleResponse 
            {
                Aisle = g.First().Aisle.ToResponse(),
                Items = g.Select(x => x.Item.ToResponse()).ToList()
            })
            .ToList();

        ItemAisleResponse otherItems = new()
        {
            Aisle = null,
            Items = db.Items
                .Where(i => !items.Any(ix => ix.ItemId == i.ItemId))
                .Select(i => i.ToResponse())
                .ToList()
        };

        var allItems = itemAisles
            .Append(otherItems)
            .OrderBy(a => a.Aisle?.AisleOrder ?? int.MaxValue);
        
        return Ok(allItems);
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
            return Error.NotFoundItem;
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
            return Error.NotFoundItem;
        }
        
        Store? s = db.Stores
            .Include(s => s.Aisles)
            .FirstOrDefault(s => s.StoreId == storeId);
        if (s == null)
        {
            return Error.NotFoundStore;
        }

        if (db.Aisles.All(a => a.AisleId != itemAisleLocChangeRequest.AisleId))
        {
            return Error.NotFoundAisle;
        }
        
        if (s.Aisles.All(a => a.AisleId != itemAisleLocChangeRequest.AisleId))
        {
            return Error.BadRequestAisleNotUnderStore;
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
            return Error.NotFoundItem;
        }

        db.Items.Remove(i);
        db.SaveChanges();
        
        return NoContent();
    }
}