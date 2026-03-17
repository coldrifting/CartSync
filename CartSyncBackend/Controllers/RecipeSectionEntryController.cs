using System.ComponentModel.DataAnnotations;
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
[Tags("Recipes")]
public class RecipeSectionEntryController(CartSyncContext db) : ControllerCore
{
    [HttpPost]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/add")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Add(Ulid recipeId, Ulid recipeSectionId, [Required] RecipeSectionEntryAddRequest recipeSectionEntryAddRequest)
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

        RecipeSectionEntry entry = new()
        {
            RecipeSectionId = recipeSection.RecipeSectionId,
            SortOrder = recipeSection.RecipeSectionEntries.Count,
            Item = item,
            Prep = prep,
            Amount = recipeSectionEntryAddRequest.Amount
        };

        recipeSection.RecipeSectionEntries.Add(entry);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPatch]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/{recipeSectionEntryId}/edit")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid recipeId, Ulid recipeSectionId, Ulid recipeSectionEntryId, [FromBody] JsonPatchDocument<RecipeSectionEntryEditRequest> recipeSectionEntryPatch)
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
        
        return NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/entries/{recipeSectionEntryId}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid recipeId, Ulid recipeSectionId, Ulid recipeSectionEntryId)
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
        
        return NoContent();
    }
}