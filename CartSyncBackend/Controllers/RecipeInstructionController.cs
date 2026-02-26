using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
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
    public IActionResult Add([Required] Ulid recipeId, [FromBody] RecipeInstructionAddRequest recipeInstructionAddRequest)
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

        Recipe? recipe = db.Recipes
            .Include(r => r.RecipeInstructions)
            .FirstOrDefault(r => r.RecipeId == recipeId);
        if (recipe == null)
        {
            return Error.NotFoundRecipe;
        }
        
        recipe.RecipeInstructions.Add(new RecipeInstruction
        {
            RecipeId = recipeId,
            RecipeInstructionContent = recipeInstructionAddRequest.RecipeInstructionContent,
            IsImage = recipeInstructionAddRequest.IsImage,
            RecipeInstructionIndex = recipe.RecipeInstructions.Count
        });
        
        db.SaveChanges();
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public IActionResult Edit([Required] Ulid recipeInstructionId, [FromBody] RecipeInstructionEditRequest recipeInstructionEditRequest)
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

        RecipeInstruction? recipeInstruction = db.RecipeInstructions.Find(recipeInstructionId);
        if (recipeInstruction == null)
        {
            return Error.NotFoundRecipeInstruction;
        }

        recipeInstruction.RecipeInstructionContent = recipeInstructionEditRequest.RecipeInstructionContent ?? recipeInstruction.RecipeInstructionContent;
        recipeInstruction.IsImage = recipeInstructionEditRequest.IsImage ?? recipeInstruction.IsImage;
        
        if (recipeInstructionEditRequest.RecipeInstructionIndex is { } newIndex)
        {
            int oldIndex = recipeInstruction.RecipeInstructionIndex;

            if (newIndex != oldIndex)
            {
                // Insert at correct sorting index
                Recipe? recipe = db.Recipes
                    .Include(r => r.RecipeInstructions)
                    .FirstOrDefault(r => r.RecipeId == recipeInstruction.RecipeId);

                if (recipe != null)
                {
                    RecipeInstruction[] instructions = recipe.RecipeInstructions.OrderBy(ri => ri.RecipeInstructionIndex).ToArray();

                    int[] indices = SortHelper.Reorder(instructions.Length, oldIndex, newIndex);
                    for (int i = 0; i < instructions.Length; i++)
                    {
                        instructions[indices[i]].RecipeInstructionIndex = i;
                    }
                }
            }
        }
        
        db.SaveChanges();
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public IActionResult Delete([Required] Ulid recipeInstructionId)
    {
        if (!ModelState.IsValid && recipeInstructionId == Ulid.Empty)
        {
            return Error.BadRequestRecipeInstructionIdInvalid;
        }

        RecipeInstruction? recipeInstruction = db.RecipeInstructions.Find(recipeInstructionId);
        if (recipeInstruction == null)
        {
            return Error.NotFoundRecipeInstruction;
        }

        db.RecipeInstructions.Remove(recipeInstruction);
        
        // Normalize order index
        if (recipeInstruction.RecipeInstructionIndex != 0)
        {
            Recipe? recipe = db.Recipes
                .Include(r => r.RecipeInstructions)
                .FirstOrDefault(r => r.RecipeId == recipeInstruction.RecipeId);

            if (recipe != null)
            {
                // Normalize order index
                int index = 0;
                foreach (RecipeInstruction ri in recipe.RecipeInstructions.OrderBy(ri => ri.RecipeInstructionIndex))
                {
                    ri.RecipeInstructionIndex = index++;
                }
            }
        }
        
        return NoContent();
    }
}