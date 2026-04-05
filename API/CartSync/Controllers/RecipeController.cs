using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Recipes")]
public class RecipeController(CartSyncContext context) : ControllerCore(context)
{
    [HttpGet]
    [Route("/api/recipes")]
    public async Task<Ok<List<RecipeMinimalResponse>>> All()
    {
        List<RecipeMinimalResponse> recipes = await Db.Recipes
            .Include(r => r.Steps)
            .Include(r => r.Sections)
            .ThenInclude(rs => rs.Entries)
            .ThenInclude(rs => rs.Prep)
            .Select(Recipe.ToMinimalResponse)
            .OrderBy(r => r.RecipeName)
            .ToListAsync();
        
        return TypedResults.Ok(recipes);
    }

    [HttpGet]
    [Route("/api/recipes/{recipeId}")]
    public async Task<Results<Ok<RecipeResponse>, BadRequest<Error>, NotFound<Error>>> Details(Ulid recipeId)
    {
        RecipeResponse? recipe = await Db.Recipes
            .Include(r => r.Steps)
            .Include(r => r.Sections)
            .ThenInclude(rs => rs.Entries)
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
        
        Db.Add(recipe);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/{recipe.RecipeId}", recipe.ToNewResponse);
    }

    [HttpPatch]
    [Route("/api/recipes/{recipeId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid recipeId, [Required] JsonPatchDocument<RecipeEditRequest> recipeEditPatch)
    {
        Recipe? recipe = await Db.Recipes.FindAsync(recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }
        
        if (!TryGetEditObject(recipe, recipeEditPatch, out RecipeEditRequest? recipeEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        recipe.UpdateFromEditRequest(recipeEdit);
        await Db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    [HttpDelete]
    [Route("/api/recipes/{recipeId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeId)
    {
        Recipe? recipe = await Db.Recipes.FindAsync(recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        Db.Recipes.Remove(recipe);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
}