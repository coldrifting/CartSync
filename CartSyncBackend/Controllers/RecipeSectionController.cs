using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes - Sections")]
[Route("/api/recipes/sections/[action]")]
public class RecipeSectionController(CartSyncContext db) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Add([Required] Ulid recipeId, [Required] string recipeSectionName)
    {
        if (!ModelState.IsValid && recipeId == Ulid.Empty)
        {
            return Error.BadRequestRecipeIdInvalid;
        }

        if (!ModelState.IsValid || recipeSectionName.Length == 0)
        {
            return Error.BadRequestRecipeSectionNameInvalid;
        }
        
        Recipe? recipe = await db.Recipes
            .Include(r => r.RecipeSections)
            .GetAsync(recipeId);
        if (recipe == null)
        {
            return Error.NotFoundRecipe;
        }
        
        recipe.RecipeSections.Add(new RecipeSection
        {
            RecipeId = recipeId,
            RecipeSectionName = recipeSectionName,
            SortOrder = recipe.RecipeSections.Count
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit([Required] Ulid recipeSectionId, [FromBody] RecipeSectionEditRequest recipeSectionEditRequest)
    {
        switch (ModelState.IsValid)
        {
            case false when recipeSectionId == Ulid.Empty:
                return Error.BadRequestRecipeSectionIdInvalid;
            case false:
            {
                List<string> errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            
                return Error.BadRequestRecipeSectionEditRequestInvalid(errors);
            }
        }

        RecipeSection? recipeSection = await db.RecipeSections
            .Include(recipeSection => recipeSection.Recipe)
            .ThenInclude(recipe => recipe.RecipeSections)
            .GetAsync(recipeSectionId);
        if (recipeSection == null)
        {
            return Error.NotFoundRecipeSection;
        }

        recipeSection.RecipeSectionName = recipeSectionEditRequest.RecipeSectionName ?? recipeSection.RecipeSectionName;

        if (recipeSectionEditRequest.SortOrder is { } newIndex)
        {
            int oldIndex = recipeSection.SortOrder;
            recipeSection.Recipe.RecipeSections.Reorder(oldIndex, newIndex);
        }
        
        await db.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete([Required] Ulid recipeSectionId)
    {
        if (!ModelState.IsValid && recipeSectionId == Ulid.Empty)
        {
            return Error.BadRequestRecipeSectionIdInvalid;
        }

        RecipeSection? recipeSection = await db.RecipeSections
            .Include(recipeSection => recipeSection.Recipe)
            .ThenInclude(recipe => recipe.RecipeSections)
            .GetAsync(recipeSectionId);
        if (recipeSection == null)
        {
            return Error.NotFoundRecipeSection;
        }

        db.RecipeSections.Remove(recipeSection);
        
        // Refresh Sort Order
        recipeSection.Recipe.RecipeSections.RefreshOrder();

        await db.SaveChangesAsync();
        
        return NoContent();
    }
}