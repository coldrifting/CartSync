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
public class RecipeStepsController(CartSyncContext context) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/recipes/steps/add")]
    public async Task<Results<Created<RecipeStepResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Add([Required] Ulid recipeId, RecipeStepAddRequest recipeStepAddRequest)
    {
        Recipe? recipe = await Db.Recipes
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(recipe => recipe.RecipeId == recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        RecipeStep recipeStep = new()
        {
            RecipeId = recipeId,
            RecipeStepContent = recipeStepAddRequest.Content,
            IsImage = recipeStepAddRequest.IsImage,
            SortOrder = recipe.Steps.Count
        };
        
        Db.Add(recipeStep);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/steps/{recipeStep.RecipeStepId}", RecipeStepResponse.FromNewEntity(recipeStep));
    }
    
    [HttpPatch]
    [Route("/api/recipes/steps/{recipeStepId}/edit")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Edit(Ulid recipeStepId, JsonPatchDocument<RecipeStepEditRequest> jsonPatch)
    {
        RecipeStep? step = await Db.RecipeSteps
            .Include(step => step.Recipe)
            .ThenInclude(recipe => recipe.Steps)
            .FirstOrDefaultAsync(step => step.RecipeStepId == recipeStepId);
        if (step == null)
        {
            return RecipeStep.NotFound(recipeStepId);
        }

        if (!step.TryGetPatch(ModelState, jsonPatch, out RecipeStepEditRequest patch))
        {
            return ErrorResponse.BadRequestPatchInvalid(ModelState);
        }

        step.ApplyPatch(patch);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/steps/{recipeStepId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Delete(Ulid recipeStepId)
    {
        RecipeStep? recipeStep = await Db.RecipeSteps
            .Include(recipeStep => recipeStep.Recipe)
            .ThenInclude(recipe => recipe.Steps)
            .FirstOrDefaultAsync(recipeStep => recipeStep.RecipeStepId == recipeStepId);
        if (recipeStep == null)
        {
            return RecipeStep.NotFound(recipeStepId);
        }

        Db.RecipeSteps.Remove(recipeStep);
        
        // Refresh Sort Order
        Sort.RefreshOrder(recipeStep.Recipe.Steps);

        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}