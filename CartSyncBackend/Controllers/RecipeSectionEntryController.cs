using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes")]
public class RecipeSectionEntryController(CartSyncContext db) : ControllerCore
{
    [HttpPost]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/add")]
    public async Task<Results<Created<RecipeSectionEntryResponse>, BadRequest<Error>, NotFound<Error>>> Add(Ulid recipeId, Ulid recipeSectionId, [Required] RecipeSectionEntryAddRequest recipeSectionEntryAddRequest)
    {
        RecipeSection? recipeSection = await db.RecipeSections
            .Include(rs => rs.RecipeSectionEntries)
            .FirstOrDefaultAsync(recipeSection => recipeSection.RecipeSectionId == recipeSectionId);
        if (recipeSection == null)
        {
            return RecipeSection.NotFound(recipeSectionId);
        }
        
        if (recipeSection.RecipeId != recipeId)
        {
            return RecipeSection.NotFoundUnderRecipe(recipeSectionId, recipeId);
        }

        Ulid itemId = recipeSectionEntryAddRequest.ItemId;
        Item? item = await db.Items.FindAsync(itemId);
        if (item == null)
        {
            return Item.NotFound(itemId);
        }

        Prep? prep;
        if (recipeSectionEntryAddRequest.PrepId is { } prepId)
        {
            prep = await db.Preps.FindAsync(prepId);
            if (prep == null)
            {
                return Prep.NotFound(prepId);
            }
        }
        else
        {
            prep = null;
        }

        RecipeSectionEntry recipeSectionEntry = new()
        {
            RecipeSectionId = recipeSection.RecipeSectionId,
            SortOrder = recipeSection.RecipeSectionEntries.Count,
            Item = item,
            Prep = prep,
            Amount = recipeSectionEntryAddRequest.Amount
        };

        await db.RecipeSectionEntries.AddAsync(recipeSectionEntry);
        await db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/{recipeSectionEntry.RecipeSectionEntryId}", recipeSectionEntry.ToNewResponse);
    }
    
    [HttpPatch]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/{recipeSectionEntryId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid recipeId, Ulid recipeSectionId, Ulid recipeSectionEntryId, [FromBody] JsonPatchDocument<RecipeSectionEntryEditRequest> recipeSectionEntryPatch)
    {
        RecipeSectionEntry? recipeSectionEntry = await db.RecipeSectionEntries
            .Include(recipeSectionEntry => recipeSectionEntry.RecipeSection)
            .ThenInclude(recipeSection => recipeSection.RecipeSectionEntries)
            .FirstOrDefaultAsync(rse => rse.RecipeSectionEntryId == recipeSectionEntryId);
        if (recipeSectionEntry == null)
        {
            return RecipeSectionEntry.NotFound(recipeSectionEntryId);
        }

        if (recipeSectionEntry.RecipeSectionId != recipeSectionId)
        {
            return RecipeSectionEntry.NotFoundUnderRecipeSection(recipeSectionEntryId,  recipeSectionId);
        }

        if (recipeSectionEntry.RecipeSection.RecipeId != recipeId)
        {
            return RecipeSectionEntry.NotFoundUnderRecipe(recipeSectionEntryId, recipeId);
        }
        
        if (!TryGetEditObject(recipeSectionEntry, recipeSectionEntryPatch, out RecipeSectionEntryEditRequest? recipeSectionEntryEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        if (await db.Items.FindAsync(recipeSectionEntry.ItemId) == null)
        {
            return Item.NotFound(recipeSectionEntry.ItemId);
        }

        if (recipeSectionEntryEdit.PrepId is { } prepId)
        {
            if (await db.Preps.FindAsync(prepId) == null)
            {
                return Prep.NotFound(prepId);
            }
        }
        
        recipeSectionEntry.UpdateFromEditRequest(recipeSectionEntryEdit);
        await db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/{recipeSectionEntryId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeId, Ulid recipeSectionId, Ulid recipeSectionEntryId)
    {
        RecipeSectionEntry? recipeSectionEntry = await db.RecipeSectionEntries
            .Include(recipeSectionEntry => recipeSectionEntry.RecipeSection)
            .ThenInclude(recipeSection => recipeSection.RecipeSectionEntries)
            .FirstOrDefaultAsync(recipeSectionEntry => recipeSectionEntry.RecipeSectionEntryId == recipeSectionEntryId);
        if (recipeSectionEntry == null)
        {
            return RecipeSectionEntry.NotFound(recipeSectionEntryId);
        }
        
        if (recipeSectionEntry.RecipeSectionId != recipeSectionId)
        {
            return RecipeSectionEntry.NotFoundUnderRecipeSection(recipeSectionEntryId,  recipeSectionId);
        }

        if (recipeSectionEntry.RecipeSection.RecipeId != recipeId)
        {
            return RecipeSectionEntry.NotFoundUnderRecipe(recipeSectionEntryId, recipeId);
        }
        
        db.RecipeSectionEntries.Remove(recipeSectionEntry);
        
        // Refresh Sort Order
        recipeSectionEntry.RecipeSection.RecipeSectionEntries.RefreshOrder();

        await db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}