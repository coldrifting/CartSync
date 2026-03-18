using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes")]
public class RecipeController(CartSyncContext db) : ControllerCore
{
    [HttpGet]
    [Route("/api/recipes")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RecipeResponse>))]
    public async Task<IActionResult> All()
    {
        List<RecipeResponse> recipes = await db.Recipes
            .Include(r => r.RecipeInstructions)
            .Include(r => r.RecipeSections)
            .ThenInclude(rs => rs.RecipeSectionEntries)
            .ThenInclude(rs => rs.Prep)
            .Select(Recipe.ToResponse)
            .OrderBy(r => r.RecipeName)
            .ToListAsync();
        
        return Ok(recipes);
    }

    [HttpGet]
    [Route("/api/recipes/{recipeId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecipeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Details(Ulid recipeId)
    {
        RecipeResponse? recipe = await db.Recipes
            .Include(r => r.RecipeInstructions)
            .Include(r => r.RecipeSections)
            .ThenInclude(rs => rs.RecipeSectionEntries)
            .ThenInclude(rs => rs.Prep)
            .Select(Recipe.ToResponse)
            .FirstOrDefaultAsync(r => r.RecipeId == recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        return Ok(recipe);
    }

    [HttpPost]
    [Route("/api/recipes/add")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RecipeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<IActionResult> Add([Required] string recipeName)
    {
        Recipe recipe = new()
        {
            RecipeName = recipeName
        };
        
        await db.Recipes.AddAsync(recipe);
        await db.SaveChangesAsync();
        
        return Created($"/api/recipes/{recipe.RecipeId}", recipe.ToNewResponse);
    }

    [HttpPatch]
    [Route("/api/recipes/{recipeId}/edit")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid recipeId, [Required] JsonPatchDocument<RecipeEditRequest> recipeEditPatch)
    {
        Recipe? recipe = await db.Recipes.FindAsync(recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }
        
        if (!TryGetEditObject(recipe, recipeEditPatch, out RecipeEditRequest? recipeEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        recipe.UpdateFromEditRequest(recipeEdit);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete]
    [Route("/api/recipes/{recipeId}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid recipeId)
    {
        Recipe? recipe = await db.Recipes.FindAsync(recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
}