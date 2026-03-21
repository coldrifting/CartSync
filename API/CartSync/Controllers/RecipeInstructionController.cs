using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Recipes")]
public class RecipeInstructionController(CartSyncContext db) : ControllerCore
{
    [HttpPost]
    [Route("/api/recipes/{recipeId}/instructions/add")]
    public async Task<Results<Created<RecipeInstructionResponse>, BadRequest<Error>, NotFound<Error>>> Add(Ulid recipeId, [FromBody] RecipeInstructionAddRequest recipeInstructionAddRequest)
    {
        Recipe? recipe = await db.Recipes
            .Include(r => r.RecipeInstructions)
            .FirstOrDefaultAsync(recipe => recipe.RecipeId == recipeId);
        if (recipe == null)
        {
            return Recipe.NotFound(recipeId);
        }

        RecipeInstruction recipeInstruction = new()
        {
            RecipeId = recipeId,
            RecipeInstructionContent = recipeInstructionAddRequest.RecipeInstructionContent,
            IsImage = recipeInstructionAddRequest.IsImage,
            SortOrder = recipe.RecipeInstructions.Count
        };
        
        await db.RecipeInstructions.AddAsync(recipeInstruction);
        await db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/{recipe.RecipeId}/instructions/{recipeInstruction.RecipeInstructionId}", recipeInstruction.ToNewResponse);
    }
    
    [HttpPut]
    [Route("/api/recipes/{recipeId}/instructions/{recipeInstructionId}/edit")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid recipeId, Ulid recipeInstructionId, [FromBody] JsonPatchDocument<RecipeInstructionEditRequest> recipeInstructionPatch)
    {
        RecipeInstruction? recipeInstruction = await db.RecipeInstructions
            .Include(recipeInstruction => recipeInstruction.Recipe)
            .ThenInclude(recipe => recipe.RecipeInstructions)
            .FirstOrDefaultAsync(recipeInstruction => recipeInstruction.RecipeInstructionId == recipeInstructionId);
        if (recipeInstruction == null)
        {
            return RecipeInstruction.NotFound(recipeInstructionId);
        }

        if (recipeInstruction.RecipeId != recipeId)
        {
            return RecipeInstruction.NotFoundUnderRecipe(recipeInstructionId, recipeId);
        }

        if (!TryGetEditObject(recipeInstruction, recipeInstructionPatch, out RecipeInstructionEditRequest? recipeInstructionEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        recipeInstruction.UpdateFromEditRequest(recipeInstructionEdit);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/{recipeId}/instructions/{recipeInstructionId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeId, Ulid recipeInstructionId)
    {
        RecipeInstruction? recipeInstruction = await db.RecipeInstructions
            .Include(recipeInstruction => recipeInstruction.Recipe)
            .ThenInclude(recipe => recipe.RecipeInstructions)
            .FirstOrDefaultAsync(recipeInstruction => recipeInstruction.RecipeInstructionId == recipeInstructionId);
        if (recipeInstruction == null)
        {
            return RecipeInstruction.NotFound(recipeInstructionId);
        }

        if (recipeInstruction.RecipeId != recipeId)
        {
            return RecipeInstruction.NotFoundUnderRecipe(recipeInstructionId, recipeId);
        }

        db.RecipeInstructions.Remove(recipeInstruction);
        
        // Refresh Sort Order
        recipeInstruction.Recipe.RecipeInstructions.RefreshOrder();

        await db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}