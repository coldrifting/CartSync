using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes - Instructions")]
[Route("/api/recipes/instructions/[action]")]
public class RecipeInstructionController(CartSyncContext db) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Add([Required] Ulid recipeId, [FromBody] RecipeInstructionAddRequest recipeInstructionAddRequest)
    {
        switch (ModelState.IsValid)
        {
            case false when recipeId == Ulid.Empty:
                return Error.BadRequestRecipeIdInvalid;
            case false:
            {
                List<string> errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            
                return Error.BadRequestRecipeInstructionAddRequestInvalid(errors);
            }
        }

        Recipe? recipe = await db.Recipes
            .Include(r => r.RecipeInstructions)
            .GetAsync(recipeId);
        if (recipe == null)
        {
            return Error.NotFoundRecipe;
        }
        
        recipe.RecipeInstructions.Add(new RecipeInstruction
        {
            RecipeId = recipeId,
            RecipeInstructionContent = recipeInstructionAddRequest.RecipeInstructionContent,
            IsImage = recipeInstructionAddRequest.IsImage,
            SortOrder = recipe.RecipeInstructions.Count
        });
        
        await db.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit([Required] Ulid recipeInstructionId, [FromBody] RecipeInstructionEditRequest recipeInstructionEditRequest)
    {
        switch (ModelState.IsValid)
        {
            case false when recipeInstructionId == Ulid.Empty:
                return Error.BadRequestRecipeInstructionIdInvalid;
            case false:
            {
                List<string> errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            
                return Error.BadRequestRecipeInstructionEditRequestInvalid(errors);
            }
        }

        RecipeInstruction? recipeInstruction = await db.RecipeInstructions
            .Include(recipeInstruction => recipeInstruction.Recipe)
            .ThenInclude(recipe => recipe.RecipeInstructions)
            .GetAsync(recipeInstructionId);
        if (recipeInstruction == null)
        {
            return Error.NotFoundRecipeInstruction;
        }

        recipeInstruction.RecipeInstructionContent = recipeInstructionEditRequest.RecipeInstructionContent ?? recipeInstruction.RecipeInstructionContent;
        recipeInstruction.IsImage = recipeInstructionEditRequest.IsImage ?? recipeInstruction.IsImage;
        
        if (recipeInstructionEditRequest.SortOrder is { } newIndex)
        {
            int oldIndex = recipeInstruction.SortOrder;
            recipeInstruction.Recipe.RecipeInstructions.Reorder(oldIndex, newIndex);
        }
        
        await db.SaveChangesAsync();
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete([Required] Ulid recipeInstructionId)
    {
        if (!ModelState.IsValid && recipeInstructionId == Ulid.Empty)
        {
            return Error.BadRequestRecipeInstructionIdInvalid;
        }

        RecipeInstruction? recipeInstruction = await db.RecipeInstructions
            .Include(recipeInstruction => recipeInstruction.Recipe)
            .ThenInclude(recipe => recipe.RecipeInstructions)
            .GetAsync(recipeInstructionId);
        if (recipeInstruction == null)
        {
            return Error.NotFoundRecipeInstruction;
        }

        db.RecipeInstructions.Remove(recipeInstruction);
        
        // Refresh Sort Order
        recipeInstruction.Recipe.RecipeInstructions.RefreshOrder();

        await db.SaveChangesAsync();
        
        return NoContent();
    }
}