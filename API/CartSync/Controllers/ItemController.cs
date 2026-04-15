using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Objects;
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
    public async Task<Results<Ok<ReadOnlyList<ItemResponse>>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> All()
    {
        ReadOnlyList<ItemResponse> allItems = await Db.Items
                .Include(i => i.Preps)
                .Include(i => i.Aisles)
                .Select(ItemResponse.FromEntity)
                .OrderBy(item => item.Name)
                .ThenBy(item => item.Id)
                .ToReadOnlyListAsync();

        return TypedResults.Ok(allItems);
    }
    
    [HttpGet]
    [Route("/api/items/{itemId}/usages")]
    public async Task<Results<Ok<ItemUsagesResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Usages(Ulid itemId)
    {
        ItemUsagesResponse? itemUsages = await Db.Items
            .Include(item => item.RecipeSectionEntries)
            .ThenInclude(entry => entry.RecipeSection)
            .ThenInclude(section => section.Recipe)
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
        ItemResponse? itemResponse = await Db.Items
            .Include(i => i.Preps)
            .Include(i => i.Aisles)
            .Select(ItemResponse.FromEntity)
            .FirstOrDefaultAsync(item => item.Id == itemId);

        if (itemResponse == null)
        {
            return Item.NotFound(itemId);
        }

        return TypedResults.Ok(itemResponse);
    }
    
    [HttpGet]
    [Route("/api/items/{itemId}/preps")]
    public async Task<Results<Ok<ItemPrepDetailsResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> PrepDetails(Ulid itemId)
    {
        Item? item = await Db.Items
            .Include(item => item.Preps)
            .FirstOrDefaultAsync(item => item.ItemId == itemId);
        if (item is null)
        {
            return Item.NotFound(itemId);
        }

        ItemMinimalResponse itemMinimal = ItemMinimalResponse.FromNewEntity(item);
        Dictionary<Ulid, ItemPrepResponse> allPreps = Db.Preps
            .Select(p => new ItemPrepResponse
            {
                Id = p.PrepId,
                Name = p.PrepName,
                IsSelected = false
            })
            .ToDictionary(p => p.Id, p => p);

        foreach (Prep prep in item.Preps)
        {
            allPreps[prep.PrepId] = new ItemPrepResponse
            {
                Id = prep.PrepId,
                Name = prep.PrepName,
                IsSelected = true
            };
        }

        ItemPrepDetailsResponse result = new()
        {
            Item = itemMinimal,
            AllPreps = allPreps.Values
                .OrderBy(itemPrep => itemPrep.Name)
                .ThenBy(itemPrep => itemPrep.Id)
                .ToReadOnlyList(),
        };

        return TypedResults.Ok(result);
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
    
    [HttpPut]
    [Route("/api/items/{itemId}/preps")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> EditPreps(Ulid itemId, ItemPrepEditRequest edit)
    {
        Item? item = Db.Items
            .Include(item => item.Preps)
            .FirstOrDefault(item => item.ItemId == itemId);
        if (item == null)
        {
            return Item.NotFound(itemId);
        }

        item.Preps.Clear();
        foreach (Ulid prepId in edit.PrepIds)
        {
            Prep? prep = await Db.Preps.FindAsync(prepId);
            if (prep is null)
            {
                return Prep.NotFound(prepId);
            }
            item.Preps.Add(prep);
        }

        await Db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    [HttpDelete]
    [Route("/api/items/{itemId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Delete(Ulid itemId)
    {
        Item? item = await Db.Items.FindAsync(itemId);
        if (item == null)
        {
            return Item.NotFound(itemId);
        }

        Db.Items.Remove(item);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}