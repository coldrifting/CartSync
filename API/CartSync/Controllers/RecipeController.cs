using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Objects;
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
    public async Task<Ok<ReadOnlyList<RecipeMinimalResponse>>> All()
    {
        ReadOnlyList<RecipeMinimalResponse> recipes = await Db.Recipes
            .Include(recipe => recipe.Steps)
            .Include(recipe => recipe.Sections)
            .ThenInclude(section => section.Entries)
            .ThenInclude(entry => entry.Prep)
            .Select(RecipeMinimalResponse.FromEntity)
            .OrderBy(recipe => recipe.Name)
            .ThenBy(recipe => recipe.Id)
            .ToReadOnlyListAsync();
        
        return TypedResults.Ok(recipes);
    }

    [HttpGet]
    [Route("/api/recipes/{recipeId}")]
    public async Task<Results<Ok<RecipeResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Details(Ulid recipeId)
    {
        RecipeResponse? recipe = await Db.Recipes
            .Include(recipe => recipe.Steps)
            .Include(recipe => recipe.Sections)
            .ThenInclude(section => section.Entries)
            .ThenInclude(entry => entry.Prep)
            .Select(RecipeResponse.FromEntity)
            .FirstOrDefaultAsync(recipe => recipe.Id == recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        return TypedResults.Ok(recipe);
    }

    [HttpPost]
    [Route("/api/recipes/add")]
    public async Task<Results<Created<RecipeMinimalResponse>, BadRequest<ErrorResponse>>> Add(AddRequest addRequest)
    {
        Recipe recipe = new()
        {
            RecipeName = addRequest.Name
        };
        
        Db.Add(recipe);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/{recipe.RecipeId}", RecipeMinimalResponse.FromNewEntity(recipe));
    }

    [HttpPatch]
    [Route("/api/recipes/{recipeId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Edit(Ulid recipeId, [Required] JsonPatchDocument<RecipeEditRequest> jsonPatch)
    {
        Recipe? recipe = await Db.Recipes.FindAsync(recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }
        
        if (!recipe.TryGetPatch(ModelState, jsonPatch, out RecipeEditRequest patch))
        {
            return ErrorResponse.BadRequestPatchInvalid(ModelState);
        }
        
        recipe.ApplyPatch(patch);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }

    [HttpDelete]
    [Route("/api/recipes/{recipeId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Delete(Ulid recipeId)
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