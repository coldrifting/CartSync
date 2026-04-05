using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Models;
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
    public async Task<Results<Created<RecipeEntryResponse>, BadRequest<Error>, NotFound<Error>, Conflict<Error>>> Add([Required] Ulid recipeSectionId, [Required] RecipeEntryAddRequest recipeEntryAddRequest)
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

        RecipeEntry recipeEntry = new()
        {
            RecipeSectionId = recipeSection.RecipeSectionId,
            ItemId = item.ItemId,
            PrepId = prep?.PrepId,
            Amount = recipeEntryAddRequest.Amount
        };

        RecipeEntry? existing = await Db.RecipeEntries.FirstOrDefaultAsync(r =>
            r.RecipeSectionId == recipeEntry.RecipeSectionId &&
            r.ItemId == recipeEntry.ItemId &&
            r.PrepId == recipeEntry.PrepId);

        if (existing is not null)
        {
            return RecipeEntry.AlreadyExists(recipeEntry.ItemId, recipeEntry.PrepId);
        }
        
        Db.Add(recipeEntry);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/entries/{recipeEntry.RecipeEntryId}", recipeEntry.ToNewResponse);
    }
    
    [HttpPatch]
    [Route("/api/recipes/entries/{recipeEntryId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>, Conflict<Error>>> Edit(Ulid recipeEntryId, [FromBody] JsonPatchDocument<RecipeEntryEditRequest> recipeEntryPatch)
    {
        RecipeEntry? recipeEntry = await Db.RecipeEntries
            .Include(recipeEntry => recipeEntry.RecipeSection)
            .ThenInclude(recipeSection => recipeSection.Entries)
            .FirstOrDefaultAsync(re => re.RecipeEntryId == recipeEntryId);
        if (recipeEntry == null)
        {
            return RecipeEntry.NotFound(recipeEntryId);
        }
        
        if (!TryGetEditObject(recipeEntry, recipeEntryPatch, out RecipeEntryEditRequest? recipeSectionEntryEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        if (recipeSectionEntryEdit.PrepId is { } prepId)
        {
            if (await Db.Preps.FindAsync(prepId) == null)
            {
                return Prep.NotFound(prepId);
            }
            List<Ulid> itemPreps = Db.ItemPreps.Where(ip => ip.ItemId == recipeEntry.ItemId).Select(ip => ip.Prep).Select(p => p.PrepId).ToList();
            if (!itemPreps.Contains(prepId))
            {
                return Prep.NotFoundUnder(prepId, recipeEntry.ItemId);
            }
        }

        // Verify that changing part of the composite key will still be valid
        if (recipeSectionEntryEdit.PrepId != recipeEntry.PrepId)
        {
            RecipeEntry? existing = await Db.RecipeEntries.FirstOrDefaultAsync(r =>
                r.RecipeSectionId == recipeEntry.RecipeSectionId &&
                r.ItemId == recipeEntry.ItemId &&
                r.PrepId == recipeSectionEntryEdit.PrepId);

            if (existing is not null)
            {
                return RecipeEntry.AlreadyExists(recipeEntry.ItemId, recipeSectionEntryEdit.PrepId);
            }
        }
        
        recipeEntry.UpdateFromEditRequest(recipeSectionEntryEdit);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/entries/{recipeEntryId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeEntryId)
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