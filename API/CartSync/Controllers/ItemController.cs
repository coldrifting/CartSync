using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Models.Joins;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Items")]
public class ItemController(CartSyncContext context) : ControllerCore(context)
{
    [HttpGet]
    [Route("/api/items")]
    public async Task<Results<Ok<List<ItemByStoreResponse>>, BadRequest<Error>, NotFound<Error>>> All()
    {
        Ulid storeId = await GetSelectedStoreId();
        List<ItemByStoreResponse> allItems = await Db.Items
                .Include(i => i.Preps)
                .Include(i => i.Aisles)
                .Select(Item.ToByStoreResponse(storeId))
                .OrderBy(i => i.ItemName)
                .ThenBy(i => i.ItemId)
                .ToListAsync();

        return TypedResults.Ok(allItems);
    }
    
    [HttpGet]
    [Route("/api/items/{itemId}/usages")]
    public async Task<Results<Ok<UsageResponse>, BadRequest<Error>, NotFound<Error>>> Usages(Ulid itemId)
    {
        Item? item = await Db.Items
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
        
        return TypedResults.Ok(result);
    }
    
    [HttpPost]
    [Route("/api/items/add")]
    public async Task<Results<Created<ItemResponse>, BadRequest<Error>>> Add([FromBody] ItemAddRequest itemAddRequest)
    {
        Item item = new()
        {
            ItemName = itemAddRequest.ItemName
        };
        
        Db.Add(item);
        await Db.SaveChangesAsync();

        return TypedResults.Created($"/api/items/{item.ItemId}", item.ToNewResponse);
    }
    
    [HttpGet]
    [Route("/api/items/{itemId}")]
    public async Task<Results<Ok<ItemResponse>, BadRequest<Error>, NotFound<Error>>> Details(Ulid itemId)
    {
        ItemResponse? itemResponse = await Db.Items
            .Include(i => i.Preps)
            .Include(i => i.Aisles)
            .Select(Item.ToResponse)
            .FirstOrDefaultAsync(i => i.ItemId == itemId);

        if (itemResponse == null)
        {
            return Item.NotFound(itemId);
        }

        return TypedResults.Ok(itemResponse);
    }
    
    [HttpPatch]
    [Route("/api/items/{itemId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid itemId, [FromBody] JsonPatchDocument<ItemEditRequest> itemPatch, Ulid? storeId = null)
    {
        Item? item = await Db.Items
            .Include(i => i.Preps)
            .Include(i => i.Aisles)
            .FirstOrDefaultAsync(i => i.ItemId == itemId);
        if (item == null)
        {
            return Item.NotFound(itemId);
        }
        
        if (!TryGetEditObject(item, itemPatch, out ItemEditRequest? itemEdit, storeId))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        if (itemEdit.PrepIds.Count > 0)
        {
            List<Ulid> allPrepIds = Db.Preps.Select(p => p.PrepId).ToList();
            foreach (Ulid prepId in itemEdit.PrepIds.Where(prepId => !allPrepIds.Contains(prepId)))
            {
                return Prep.NotFound(prepId);
            }
        }
        
        if (storeId is not null)
        {
            Store? store = await Db.Stores
                    .Include(s => s.Aisles)
                    .FirstOrDefaultAsync(s => s.StoreId == storeId);
            if (store == null)
            {
                return Store.NotFound(storeId.Value);
            }
            
            Ulid? aisleId = itemEdit.AisleId;
            if (aisleId is null)
            {
                ItemAisle? itemAisle = await Db.ItemAisles.FindAsync(itemId, storeId);
                if (itemAisle != null)
                {
                    Db.ItemAisles.Remove(itemAisle);
                }
            }
            else
            {
                Aisle? aisle = await Db.Aisles.FindAsync(aisleId);
                if (aisle is null)
                {
                    return Aisle.NotFound(aisleId.Value);
                }
                
                Aisle? aisleInStore = await Db.Aisles
                    .Where(a => a.StoreId == storeId)
                    .FirstOrDefaultAsync(a => a.AisleId == aisleId.Value);
                if (aisleInStore is null)
                {
                    return Aisle.NotFoundUnderStore(aisleId.Value, storeId.Value);
                }
                
                ItemAisle? itemAisle = await Db.ItemAisles.FindAsync(itemId, storeId);
                if (itemAisle is not null)
                {
                    itemAisle.AisleId = aisleId.Value;
                }
                else
                {
                    Db.ItemAisles.Add(new ItemAisle
                    {
                        ItemId = itemId,
                        StoreId = storeId.Value,
                        AisleId = aisleId.Value,
                    });
                }
            }
        }

        item.UpdateFromEditRequest(itemEdit);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/items/{itemId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid itemId)
    {
        Item? i = await Db.Items.FindAsync(itemId);
        if (i == null)
        {
            return Item.NotFound(itemId);
        }

        Db.Items.Remove(i);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}