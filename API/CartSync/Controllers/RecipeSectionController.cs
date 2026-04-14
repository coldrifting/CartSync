using System.ComponentModel.DataAnnotations;
using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Recipes")]
public class RecipeSectionController(CartSyncContext context) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/recipes/sections/add")]
    public async Task<Results<Created<RecipeSectionResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Add([Required] Ulid recipeId, AddRequest addRequest)
    {
        Recipe? recipe = await Db.Recipes
            .Include(r => r.Sections)
            .FirstOrDefaultAsync(recipe => recipe.RecipeId == recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        RecipeSection recipeSection = new()
        {
            RecipeId = recipeId,
            RecipeSectionName = addRequest.Name,
            SortOrder = recipe.Sections.Count
        };
        
        Db.Add(recipeSection);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/sections/{recipeSection.RecipeSectionId}", RecipeSectionResponse.FromNewEntity(recipeSection));
    }
    
    [HttpPatch]
    [Route("/api/recipes/sections/{recipeSectionId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Edit(Ulid recipeSectionId, JsonPatchDocument<RecipeSectionEditRequest> jsonPatch)
    {
        RecipeSection? section = await Db.RecipeSections
            .Include(section => section.Recipe)
            .ThenInclude(recipe => recipe.Sections)
            .FirstOrDefaultAsync(section => section.RecipeSectionId == recipeSectionId);
        if (section == null)
        {
            return RecipeSection.NotFound(recipeSectionId);
        }

        if (!section.TryGetPatch(ModelState, jsonPatch, out RecipeSectionEditRequest patch))
        {
            return ErrorResponse.BadRequestPatchInvalid(ModelState);
        }

        section.ApplyPatch(patch);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/sections/{recipeSectionId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Delete(Ulid recipeSectionId)
    {
        RecipeSection? recipeSection = await Db.RecipeSections
            .Include(recipeSection => recipeSection.Recipe)
            .ThenInclude(recipe => recipe.Sections)
            .FirstOrDefaultAsync(recipeSection => recipeSection.RecipeSectionId == recipeSectionId);
        if (recipeSection == null)
        {
            return RecipeSection.NotFound(recipeSectionId);
        }

        Db.RecipeSections.Remove(recipeSection);
        
        // Refresh Sort Order
        Sort.RefreshOrder(recipeSection.Recipe.Sections);

        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}