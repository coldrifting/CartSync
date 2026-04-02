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
public class RecipeSectionController(CartSyncContext context) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/recipes/{recipeId}/sections/add")]
    public async Task<Results<Created<RecipeSectionResponse>, BadRequest<Error>, NotFound<Error>>> Add(Ulid recipeId, [FromBody] RecipeSectionAddRequest recipeSectionAddRequest)
    {
        Recipe? recipe = await Db.Recipes
            .Include(r => r.RecipeSections)
            .FirstOrDefaultAsync(recipe => recipe.RecipeId == recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        RecipeSection recipeSection = new()
        {
            RecipeId = recipeId,
            RecipeSectionName = recipeSectionAddRequest.RecipeSectionName,
            SortOrder = recipe.RecipeSections.Count
        };
        
        Db.Add(recipeSection);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/{recipeId}/sections/{recipeSection.RecipeSectionId}", recipeSection.ToNewResponse);
    }
    
    [HttpPatch]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid recipeId, Ulid recipeSectionId, [FromBody] JsonPatchDocument<RecipeSectionEditRequest> recipeSectionEditPatch)
    {
        RecipeSection? recipeSection = await Db.RecipeSections
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
        await Db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/{recipeId}/sections/{recipeSectionId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeId, Ulid recipeSectionId)
    {
        RecipeSection? recipeSection = await Db.RecipeSections
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

        Db.RecipeSections.Remove(recipeSection);
        
        // Refresh Sort Order
        recipeSection.Recipe.RecipeSections.RefreshOrder();

        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}