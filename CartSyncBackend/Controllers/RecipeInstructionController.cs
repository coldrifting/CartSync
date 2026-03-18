using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes")]
public class RecipeInstructionController(CartSyncContext db) : ControllerCore
{
    [HttpPost]
    [Route("/api/recipes/{recipeId}/instructions/add")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RecipeInstructionResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Add(Ulid recipeId, [FromBody] RecipeInstructionAddRequest recipeInstructionAddRequest)
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
        
        return Created($"/api/recipes/{recipe.RecipeId}/instructions/{recipeInstruction.RecipeInstructionId}", recipeInstruction.ToNewResponse);
    }
    
    [HttpPut]
    [Route("/api/recipes/{recipeId}/instructions/{recipeInstructionId}/edit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid recipeId, Ulid recipeInstructionId, [FromBody] JsonPatchDocument<RecipeInstructionEditRequest> recipeInstructionPatch)
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
        return NoContent();
    }
    
    [HttpDelete]
    [Route("/api/recipes/{recipeId}/instructions/{recipeInstructionId}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid recipeId, Ulid recipeInstructionId)
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
        
        return NoContent();
    }
}