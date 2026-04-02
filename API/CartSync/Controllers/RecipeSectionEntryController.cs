using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Recipes")]
public class RecipeSectionEntryController(CartSyncContext context) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/add")]
    public async Task<Results<Created<RecipeSectionEntryResponse>, BadRequest<Error>, NotFound<Error>>> Add(Ulid recipeId, Ulid recipeSectionId, [Required] RecipeSectionEntryAddRequest recipeSectionEntryAddRequest)
    {
        RecipeSection? recipeSection = await Db.RecipeSections
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
        Item? item = await Db.Items.FindAsync(itemId);
        if (item == null)
        {
            return Item.NotFound(itemId);
        }

        Prep? prep;
        if (recipeSectionEntryAddRequest.PrepId is { } prepId)
        {
            prep = await Db.Preps.FindAsync(prepId);
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
            Item = item,
            Prep = prep,
            Amount = recipeSectionEntryAddRequest.Amount
        };

        Db.Add(recipeSectionEntry);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/{recipeSectionEntry.RecipeSectionEntryId}", recipeSectionEntry.ToNewResponse);
    }
    
    [HttpPatch]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/{recipeSectionEntryId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid recipeId, Ulid recipeSectionId, Ulid recipeSectionEntryId, [FromBody] JsonPatchDocument<RecipeSectionEntryEditRequest> recipeSectionEntryPatch)
    {
        RecipeSectionEntry? recipeSectionEntry = await Db.RecipeSectionEntries
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

        if (await Db.Items.FindAsync(recipeSectionEntry.ItemId) == null)
        {
            return Item.NotFound(recipeSectionEntry.ItemId);
        }

        if (recipeSectionEntryEdit.PrepId is { } prepId)
        {
            if (await Db.Preps.FindAsync(prepId) == null)
            {
                return Prep.NotFound(prepId);
            }
        }
        
        recipeSectionEntry.UpdateFromEditRequest(recipeSectionEntryEdit);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/{recipeSectionEntryId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeId, Ulid recipeSectionId, Ulid recipeSectionEntryId)
    {
        RecipeSectionEntry? recipeSectionEntry = await Db.RecipeSectionEntries
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
        
        Db.RecipeSectionEntries.Remove(recipeSectionEntry);

        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}