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
public class RecipeStepsController(CartSyncContext context) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/recipes/steps/add")]
    public async Task<Results<Created<RecipeStepResponse>, BadRequest<Error>, NotFound<Error>>> Add([Required] Ulid recipeId, [FromBody] RecipeStepAddRequest recipeStepAddRequest)
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
            RecipeStepContent = recipeStepAddRequest.RecipeStepContent,
            IsImage = recipeStepAddRequest.IsImage,
            SortOrder = recipe.Steps.Count
        };
        
        Db.Add(recipeStep);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/steps/{recipeStep.RecipeStepId}", recipeStep.ToNewResponse);
    }
    
    [HttpPatch]
    [Route("/api/recipes/steps/{recipeStepId}/edit")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid recipeStepId, [FromBody] JsonPatchDocument<RecipeStepEditRequest> recipeStepPatch)
    {
        RecipeStep? recipeStep = await Db.RecipeSteps
            .Include(recipeStep => recipeStep.Recipe)
            .ThenInclude(recipe => recipe.Steps)
            .FirstOrDefaultAsync(recipeStep => recipeStep.RecipeStepId == recipeStepId);
        if (recipeStep == null)
        {
            return RecipeStep.NotFound(recipeStepId);
        }

        if (!TryGetEditObject(recipeStep, recipeStepPatch, out RecipeStepEditRequest? recipeStepEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        recipeStep.UpdateFromEditRequest(recipeStepEdit);
        await Db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/steps/{recipeStepId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeStepId)
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
        recipeStep.Recipe.Steps.RefreshOrder();

        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}