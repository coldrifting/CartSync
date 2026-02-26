using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes - Sections")]
[Route("/api/recipes/sections/[action]")]
public class RecipeSectionController(CartSyncContext db) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public IActionResult Add([Required] Ulid recipeId, [Required] string recipeSectionName)
    {
        if (!ModelState.IsValid && recipeId == Ulid.Empty)
        {
            return Error.BadRequestRecipeIdInvalid;
        }

        if (!ModelState.IsValid || recipeSectionName.Length == 0)
        {
            return Error.BadRequestRecipeSectionNameInvalid;
        }
        
        Recipe? recipe = db.Recipes
            .Include(r => r.RecipeSections)
            .FirstOrDefault(r => r.RecipeId == recipeId);
        if (recipe == null)
        {
            return Error.NotFoundRecipe;
        }
        
        recipe.RecipeSections.Add(new RecipeSection
        {
            RecipeId = recipeId,
            RecipeSectionName = recipeSectionName,
            RecipeSectionIndex = recipe.RecipeSections.Count
        });
        db.SaveChanges();
        
        return NoContent();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public IActionResult Edit([Required] Ulid recipeSectionId, [FromBody] RecipeSectionEditRequest recipeSectionEditRequest)
    {
        switch (ModelState.IsValid)
        {
            case false when recipeSectionId == Ulid.Empty:
                return Error.BadRequestRecipeSectionIdInvalid;
            case false:
            {
                List<string> errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
            
                return Error.BadRequestRecipeSectionEditRequestInvalid(errors);
            }
        }

        RecipeSection? recipeSection = db.RecipeSections.Find(recipeSectionId);
        if (recipeSection == null)
        {
            return Error.NotFoundRecipeSection;
        }

        recipeSection.RecipeSectionName = recipeSectionEditRequest.RecipeSectionName ?? recipeSection.RecipeSectionName;

        if (recipeSectionEditRequest.RecipeSectionIndex is { } newIndex)
        {
            int oldIndex = recipeSection.RecipeSectionIndex;

            if (newIndex != oldIndex)
            {
                // Insert at correct sorting index
                Recipe? recipe = db.Recipes
                    .Include(r => r.RecipeSections)
                    .FirstOrDefault(r => r.RecipeId == recipeSection.RecipeId);

                if (recipe != null)
                {
                    RecipeSection[] sections = recipe.RecipeSections.OrderBy(rs => rs.RecipeSectionIndex).ToArray();

                    int[] indices = SortHelper.Reorder(sections.Length, oldIndex, newIndex);
                    for (int i = 0; i < sections.Length; i++)
                    {
                        sections[indices[i]].RecipeSectionIndex = i;
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
    public IActionResult Delete([Required] Ulid recipeSectionId)
    {
        if (!ModelState.IsValid && recipeSectionId == Ulid.Empty)
        {
            return Error.BadRequestRecipeSectionIdInvalid;
        }

        RecipeSection? recipeSection = db.RecipeSections.Find(recipeSectionId);
        if (recipeSection == null)
        {
            return Error.NotFoundRecipeSection;
        }

        db.RecipeSections.Remove(recipeSection);

        // Normalize order index
        if (recipeSection.RecipeSectionIndex != 0)
        {
            Recipe? recipe = db.Recipes
                .Include(r => r.RecipeSections)
                .FirstOrDefault(r => r.RecipeId == recipeSection.RecipeId);

            if (recipe != null)
            {
                // Normalize order index
                int index = 0;
                foreach (RecipeSection rs in recipe.RecipeSections.OrderBy(rs => rs.RecipeSectionIndex))
                {
                    rs.RecipeSectionIndex = index++;
                }
            }
        }

        db.SaveChanges();
        
        return NoContent();
    }
}