using System.ComponentModel.DataAnnotations;
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

[Tags("Recipes")]
public class RecipeEntryController(CartSyncContext context) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/recipes/entries/add")]
    public async Task<Results<Created<RecipeEntryResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>, Conflict<ErrorResponse>>> Add([Required] Ulid recipeSectionId, [Required] RecipeEntryAddRequest recipeEntryAddRequest)
    {
        RecipeSection? recipeSection = await Db.RecipeSections
            .Include(rs => rs.Entries)
            .FirstOrDefaultAsync(recipeSection => recipeSection.RecipeSectionId == recipeSectionId);
        if (recipeSection == null)
        {
            return RecipeSection.NotFound(recipeSectionId);
        }

        Ulid itemId = recipeEntryAddRequest.ItemId;
        Item? item = await Db.Items.FindAsync(itemId);
        if (item == null)
        {
            return Item.NotFound(itemId);
        }

        Prep? prep;
        if (recipeEntryAddRequest.PrepId is { } prepId)
        {
            prep = await Db.Preps.FindAsync(prepId);
            if (prep == null)
            {
                return Prep.NotFound(prepId);
            }
            List<Prep> itemPreps = Db.ItemPreps.Where(ip => ip.ItemId == itemId).Select(ip => ip.Prep).ToList();
            if (!itemPreps.Contains(prep))
            {
                return Prep.NotFoundUnder(prepId, itemId);
            }
        }
        else
        {
            prep = null;
        }

        RecipeEntry entry = new()
        {
            RecipeSectionId = recipeSection.RecipeSectionId,
            ItemId = item.ItemId,
            PrepId = prep?.PrepId,
            Amount = recipeEntryAddRequest.Amount
        };

        RecipeEntry? existing = await Db.RecipeEntries.FirstOrDefaultAsync(r =>
            r.RecipeSectionId == entry.RecipeSectionId &&
            r.ItemId == entry.ItemId &&
            r.PrepId == entry.PrepId);

        if (existing is not null)
        {
            return RecipeEntry.AlreadyExists(entry.ItemId, entry.PrepId);
        }
        
        Db.Add(entry);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/entries/{entry.RecipeEntryId}", RecipeEntryResponse.FromNewEntity(entry));
    }
    
    [HttpPatch]
    [Route("/api/recipes/entries/{recipeEntryId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>, Conflict<ErrorResponse>>> Edit(Ulid recipeEntryId, JsonPatchDocument<RecipeEntryEditRequest> jsonPatch)
    {
        RecipeEntry? entry = await Db.RecipeEntries
            .Include(entry => entry.RecipeSection)
            .ThenInclude(recipeSection => recipeSection.Entries)
            .FirstOrDefaultAsync(entry => entry.RecipeEntryId == recipeEntryId);
        if (entry == null)
        {
            return RecipeEntry.NotFound(recipeEntryId);
        }

        // Generate patch from input
        if (!entry.TryGetPatch(ModelState, jsonPatch, out RecipeEntryEditRequest patch))
        {
            return ErrorResponse.BadRequestPatchInvalid(ModelState);
        }
        
        // Validate input
        if (patch.PrepId is { } prepId)
        {
            if (await Db.Preps.FindAsync(prepId) == null)
            {
                return Prep.NotFound(prepId);
            }
            List<Ulid> itemPreps = Db.ItemPreps.Where(ip => ip.ItemId == entry.ItemId).Select(ip => ip.Prep).Select(p => p.PrepId).ToList();
            if (!itemPreps.Contains(prepId))
            {
                return Prep.NotFoundUnder(prepId, entry.ItemId);
            }
        }

        // Verify that changing part of the composite key will still be valid
        if (patch.PrepId != entry.PrepId)
        {
            RecipeEntry? existing = await Db.RecipeEntries.FirstOrDefaultAsync(r =>
                r.RecipeSectionId == entry.RecipeSectionId &&
                r.ItemId == entry.ItemId &&
                r.PrepId == patch.PrepId);

            if (existing is not null)
            {
                return RecipeEntry.AlreadyExists(entry.ItemId, patch.PrepId);
            }
        }
        
        // Apply patch
        entry.ApplyPatch(patch);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/entries/{recipeEntryId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Delete(Ulid recipeEntryId)
    {
        RecipeEntry? recipeEntry = await Db.RecipeEntries
            .Include(recipeEntry => recipeEntry.RecipeSection)
            .ThenInclude(recipeSection => recipeSection.Entries)
            .FirstOrDefaultAsync(recipeEntry => recipeEntry.RecipeEntryId == recipeEntryId);
        if (recipeEntry == null)
        {
            return RecipeEntry.NotFound(recipeEntryId);
        }
        
        if (recipeEntry.RecipeSection.Entries.Count == 1)
        {
            // Remove section when deleting last item
            Db.RecipeSections.Remove(recipeEntry.RecipeSection);
        }
        else
        {
            Db.RecipeEntries.Remove(recipeEntry);
        }

        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}