using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<Ok<List<RecipeResponse>>> All()
    {
        List<RecipeResponse> recipes = await db.Recipes
            .Include(r => r.RecipeInstructions)
            .Include(r => r.RecipeSections)
            .ThenInclude(rs => rs.RecipeSectionEntries)
            .ThenInclude(rs => rs.Prep)
            .Select(Recipe.ToResponse)
            .OrderBy(r => r.RecipeName)
            .ToListAsync();
        
        return TypedResults.Ok(recipes);
    }

    [HttpGet]
    [Route("/api/recipes/{recipeId}")]
    public async Task<Results<Ok<RecipeResponse>, BadRequest<Error>, NotFound<Error>>> Details(Ulid recipeId)
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

        return TypedResults.Ok(recipe);
    }

    [HttpPost]
    [Route("/api/recipes/add")]
    public async Task<Results<Created<RecipeResponse>, BadRequest<Error>>> Add(RecipeAddRequest recipeAddRequest)
    {
        Recipe recipe = new()
        {
            RecipeName = recipeAddRequest.RecipeName
        };
        
        await db.Recipes.AddAsync(recipe);
        await db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/{recipe.RecipeId}", recipe.ToNewResponse);
    }

    [HttpPatch]
    [Route("/api/recipes/{recipeId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid recipeId, [Required] JsonPatchDocument<RecipeEditRequest> recipeEditPatch)
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
        return TypedResults.NoContent();
    }

    [HttpDelete]
    [Route("/api/recipes/{recipeId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeId)
    {
        Recipe? recipe = await db.Recipes.FindAsync(recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        db.Recipes.Remove(recipe);
        await db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
}