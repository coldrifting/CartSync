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
public class RecipeSectionController(CartSyncContext db) : ControllerCore
{
    [HttpPost]
    [Route("/api/recipes/{recipeId}/sections/add")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RecipeSectionResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Add(Ulid recipeId, [Required] string recipeSectionName)
    {
        Recipe? recipe = await db.Recipes
            .Include(r => r.RecipeSections)
            .FirstOrDefaultAsync(recipe => recipe.RecipeId == recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        RecipeSection recipeSection = new()
        {
            RecipeId = recipeId,
            RecipeSectionName = recipeSectionName,
            SortOrder = recipe.RecipeSections.Count
        };
        
        await db.RecipeSections.AddAsync(recipeSection);
        await db.SaveChangesAsync();
        
        return Created($"/api/recipes/{recipeId}/sections/{recipeSection.RecipeSectionId}", recipeSection.ToNewResponse);
    }
    
    [HttpPatch]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/edit")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid recipeId, Ulid recipeSectionId, [FromBody] JsonPatchDocument<RecipeSectionEditRequest> recipeSectionEditPatch)
    {
        RecipeSection? recipeSection = await db.RecipeSections
            .Include(recipeSection => recipeSection.Recipe)
            .ThenInclude(recipe => recipe.RecipeSections)
            .FirstOrDefaultAsync(recipeSection => recipeSection.RecipeSectionId == recipeSectionId);
        if (recipeSection == null)
        {
            return RecipeSection.NotFound(recipeSectionId);
        }

        if (recipeSection.RecipeId != recipeId)
        {
            return RecipeSection.NotFoundUnderRecipe(recipeSectionId, recipeId);
        }
        
        if (!TryGetEditObject(recipeSection, recipeSectionEditPatch, out RecipeSectionEditRequest? recipeSectionEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        recipeSection.UpdateFromEditRequest(recipeSectionEdit);
        await db.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid recipeId, Ulid recipeSectionId)
    {
        RecipeSection? recipeSection = await db.RecipeSections
            .Include(recipeSection => recipeSection.Recipe)
            .ThenInclude(recipe => recipe.RecipeSections)
            .FirstOrDefaultAsync(recipeSection => recipeSection.RecipeSectionId == recipeSectionId);
        if (recipeSection == null)
        {
            return RecipeSection.NotFound(recipeSectionId);
        }

        if (recipeSection.RecipeId != recipeId)
        {
            return RecipeSection.NotFoundUnderRecipe(recipeSectionId, recipeId);
        }

        db.RecipeSections.Remove(recipeSection);
        
        // Refresh Sort Order
        recipeSection.Recipe.RecipeSections.RefreshOrder();

        await db.SaveChangesAsync();
        
        return NoContent();
    }
}