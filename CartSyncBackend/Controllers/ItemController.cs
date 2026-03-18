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
[Tags("Items")]
public class ItemController(CartSyncContext db) : ControllerCore
{
    [HttpGet]
    [Route("/api/items")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ItemResponse>))]
    public async Task<IActionResult> All(Ulid storeId)
    {
        Store? store = await db.Stores.FindAsync(storeId);
        if (store is null)
        {
            return Store.NotFound(storeId);
        }
        
        List<ItemResponse> allItems = await db.Items
                .Include(i => i.Preps)
                .Include(i => i.Aisles)
                .Select(Item.ToLocatedResponse(storeId))
                .OrderBy(i => i.ItemName)
                .ToListAsync();

        return Ok(allItems);
    }
    
    [HttpGet]
    [Route("/api/items/{itemId}/usages")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsageResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Usages(Ulid itemId)
    {
        Item? item = await db.Items
            .Include(i => i.RecipeSectionEntries)
            .ThenInclude(r => r.RecipeSection)
            .ThenInclude(r => r.Recipe)
            .FirstOrDefaultAsync(i => i.ItemId == itemId);
        if (item == null)
        {
            return Aisle.NotFound(itemId);
        }
        
        IEnumerable<Recipe> recipes = item.RecipeSectionEntries
            .Select(r => r.RecipeSection)
            .Select(r => r.Recipe)
            .Distinct()
            .OrderBy(r => r.RecipeName)
            .ThenBy(r => r.RecipeId);
        
        UsageResponse result = new();
        result.Update(recipes, r => r.RecipeId, r => r.RecipeName);
        
        return Ok(result);
    }
    
    [HttpPost]
    [Route("/api/items/add")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<IActionResult> Add([FromBody] ItemAddRequest itemAddRequest)
    {
        db.Add(new Item
        {
            ItemName = itemAddRequest.ItemName,
            ItemTemp = itemAddRequest.ItemTemp ?? ItemTemp.Ambient,
            DefaultUnitType = itemAddRequest.DefaultUnitType ?? UnitType.Count
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpGet]
    [Route("/api/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Details(Ulid itemId)
    {
        ItemResponse? itemResponse = await db.Items
            .Include(i => i.Preps)
            .Include(i => i.Aisles)
            .Select(Item.ToResponse)
            .FirstOrDefaultAsync(i => i.ItemId == itemId);

        if (itemResponse == null)
        {
            return Item.NotFound(itemId);
        }

        return Ok(itemResponse);
    }
    
    [HttpPatch]
    [Route("/api/items/{itemId}/edit")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid itemId, [FromBody] JsonPatchDocument<ItemEditRequest> itemPatch, Ulid? storeId = null)
    {
        Item? item = await db.Items
            .Include(i => i.Preps)
            .Include(i => i.Aisles)
            .FirstOrDefaultAsync(i => i.ItemId == itemId);
        if (item == null)
        {
            return Item.NotFound(itemId);
        }
        
        if (!TryGetEditObject(item, itemPatch, out ItemEditRequest? itemEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        if (itemEdit.PrepIds.Count > 0)
        {
            List<Ulid> allPrepIds = db.Preps.Select(p => p.PrepId).ToList();
            foreach (Ulid prepId in itemEdit.PrepIds.Where(prepId => !allPrepIds.Contains(prepId)))
            {
                return Prep.NotFound(prepId);
            }
        }
        
        if (storeId is not null)
        {
            Store? store = await db.Stores
                    .Include(s => s.Aisles)
                    .FirstOrDefaultAsync(s => s.StoreId == storeId);
            if (store == null)
            {
                return Store.NotFound(storeId.Value);
            }
            
            Ulid? aisleId = itemEdit.AisleId;
            if (aisleId is null)
            {
                ItemAisle? itemAisle = await db.ItemAisles.FindAsync(itemId, storeId);
                if (itemAisle != null)
                {
                    db.ItemAisles.Remove(itemAisle);
                }
            }
            else
            {
                Aisle? aisle = await db.Aisles.FindAsync(aisleId);
                if (aisle is null)
                {
                    return Aisle.NotFound(aisleId.Value);
                }
                
                Aisle? aisleInStore = await db.Aisles
                    .Where(a => a.StoreId == storeId)
                    .FirstOrDefaultAsync(a => a.AisleId == aisleId.Value);
                if (aisleInStore is null)
                {
                    return Aisle.NotFoundUnderStore(aisleId.Value, storeId.Value);
                }
                
                ItemAisle? itemAisle = await db.ItemAisles.FindAsync(itemId, storeId);
                if (itemAisle is not null)
                {
                    itemAisle.AisleId = aisleId.Value;
                }
                else
                {
                    db.ItemAisles.Add(new ItemAisle
                    {
                        ItemId = itemId,
                        StoreId = storeId.Value
                    });
                }
            }
        }

        item.UpdateFromEditRequest(itemEdit);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete]
    [Route("/api/items/{itemId}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid itemId)
    {
        Item? i = await db.Items.FindAsync(itemId);
        if (i == null)
        {
            return Item.NotFound(itemId);
        }

        db.Items.Remove(i);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}