using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
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
    public async Task<Results<Ok<List<ItemResponse>>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> All()
    {
        Ulid selectedStoreId = await GetSelectedStoreId();
        List<ItemResponse> allItems = await Db.Items
                .Include(i => i.Preps)
                .Include(i => i.Aisles)
                .Select(ItemResponse.FromEntity(selectedStoreId))
                .OrderBy(item => item.Name)
                .ThenBy(item => item.Id)
                .ToListAsync();

        return TypedResults.Ok(allItems);
    }
    
    [HttpGet]
    [Route("/api/items/{itemId}/usages")]
    public async Task<Results<Ok<ItemUsagesResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Usages(Ulid itemId)
    {
        ItemUsagesResponse? itemUsages = await Db.Items
            .Include(i => i.Preps)
            .AsSplitQuery()
            .Include(i => i.RecipeSectionEntries)
            .ThenInclude(r => r.RecipeSection)
            .ThenInclude(r => r.Recipe)
            .Select(ItemUsagesResponse.FromEntity)
            .FirstOrDefaultAsync(item => item.Id == itemId);
        if (itemUsages == null)
        {
            return Aisle.NotFound(itemId);
        }

        return TypedResults.Ok(itemUsages);
    }
    
    [HttpPost]
    [Route("/api/items/add")]
    public async Task<Results<Created<ItemMinimalResponse>, BadRequest<ErrorResponse>>> Add(AddRequest addRequest)
    {
        Item item = new()
        {
            ItemName = addRequest.Name
        };
        
        Db.Add(item);
        await Db.SaveChangesAsync();

        return TypedResults.Created($"/api/items/{item.ItemId}", ItemMinimalResponse.FromNewEntity(item));
    }
    
    [HttpGet]
    [Route("/api/items/{itemId}")]
    public async Task<Results<Ok<ItemResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Details(Ulid itemId)
    {
        Ulid selectedStoreId = await GetSelectedStoreId();
        ItemResponse? itemResponse = await Db.Items
            .Include(i => i.Preps)
            .Include(i => i.Aisles)
            .Select(ItemResponse.FromEntity(selectedStoreId))
            .FirstOrDefaultAsync(item => item.Id == itemId);

        if (itemResponse == null)
        {
            return Item.NotFound(itemId);
        }

        return TypedResults.Ok(itemResponse);
    }
    
    [HttpPatch]
    [Route("/api/items/{itemId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Edit(Ulid itemId, JsonPatchDocument<ItemEditRequest> jsonPatch)
    {
        Item? item = await Db.Items
            .Include(item => item.Preps)
            .Include(item => item.Aisles)
            .Include(item => item.ItemAisles)
            .FirstOrDefaultAsync(item => item.ItemId == itemId);
        if (item == null)
        {
            return Item.NotFound(itemId);
        }
        
        // Generate patch from input
        Ulid storeId = await GetSelectedStoreId();
        if (!item.TryGetPatch(ModelState, jsonPatch, storeId, out ItemEditRequest patch))
        {
            return ErrorResponse.BadRequestPatchInvalid(ModelState);
        }
        
        // Validate input
        if (patch.PrepIds.Count > 0)
        {
            List<Ulid> allPrepIds = Db.Preps.Select(p => p.PrepId).ToList();
            foreach (Ulid prepId in patch.PrepIds.Where(prepId => !allPrepIds.Contains(prepId)))
            {
                return Prep.NotFound(prepId);
            }
        }

        if (patch.Location is not null)
        {
            Aisle? aisle = await Db.Aisles.FindAsync(patch.Location.AisleId);
            if (aisle is null)
            {
                return Aisle.NotFound(patch.Location.AisleId);
            }
            
            Aisle? aisleInStore = await Db.Aisles
                .Where(a => a.StoreId == storeId)
                .FirstOrDefaultAsync(a => a.AisleId == patch.Location.AisleId);
            if (aisleInStore is null)
            {
                return Aisle.NotFoundUnderStore(patch.Location.AisleId, storeId);
            }
        }
        
        // Apply patch
        item.ApplyPatch(patch, storeId);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/items/{itemId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Delete(Ulid itemId)
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