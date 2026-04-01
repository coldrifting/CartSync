using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Recipes")]
public class RecipeInstructionController(CartSyncContext context) : ControllerCore(context)
{
    [HttpPost]
    [Route("/api/recipes/{recipeId}/instructions/add")]
    public async Task<Results<Created<RecipeInstructionResponse>, BadRequest<Error>, NotFound<Error>>> Add(Ulid recipeId, [FromBody] RecipeInstructionAddRequest recipeInstructionAddRequest)
    {
        Recipe? recipe = await Db.Recipes
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
        
        Db.Add(recipeInstruction);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/recipes/{recipe.RecipeId}/instructions/{recipeInstruction.RecipeInstructionId}", recipeInstruction.ToNewResponse);
    }
    
    [HttpPatch]
    [Route("/api/recipes/{recipeId}/instructions/{recipeInstructionId}/edit")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid recipeId, Ulid recipeInstructionId, [FromBody] JsonPatchDocument<RecipeInstructionEditRequest> recipeInstructionPatch)
    {
        RecipeInstruction? recipeInstruction = await Db.RecipeInstructions
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
        await Db.SaveChangesAsync();
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/{recipeId}/instructions/{recipeInstructionId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid recipeId, Ulid recipeInstructionId)
    {
        RecipeInstruction? recipeInstruction = await Db.RecipeInstructions
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

        Db.RecipeInstructions.Remove(recipeInstruction);
        
        // Refresh Sort Order
        recipeInstruction.Recipe.RecipeInstructions.RefreshOrder();

        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}