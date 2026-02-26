using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes")]
[Route("/api/recipes/[action]")]
public class RecipeController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RecipeResponse>))]
    public IActionResult All()
    {
        List<RecipeResponse> recipes = db.Recipes
            .Select(r => new RecipeResponse
            {
                RecipeId = r.RecipeId,
                RecipeName = r.RecipeName,
                Url = r.Url,
                IsPinned = r.IsPinned,
                CartAmount = r.CartAmount,
                RecipeInstructionsResponse = r.RecipeInstructions
                    .Select(rs => new RecipeInstructionResponse
                    {
                        RecipeInstructionId = rs.RecipeInstructionId,
                        RecipeInstructionIndex = rs.RecipeInstructionIndex,
                        RecipeInstructionContent = rs.RecipeInstructionContent,
                        IsImage = rs.IsImage
                    })
                    .OrderBy(rs => rs.RecipeInstructionIndex)
                    .ToList(),
                RecipeSectionsResponse = r.RecipeSections
                    .Select(rs => new RecipeSectionResponse
                    {
                        RecipeSectionId = rs.RecipeSectionId,
                        RecipeSectionIndex = rs.RecipeSectionIndex,
                        RecipeSectionName = rs.RecipeSectionName,
                        RecipeSectionEntries = rs.RecipeSectionEntries
                            .Select(re => new RecipeSectionEntryResponse
                            {
                                RecipeSectionEntryId = re.RecipeSectionEntryId,
                                RecipeSectionEntryIndex = re.RecipeSectionEntryIndex,
                                Item = new ItemResponseNoPrep
                                {
                                    ItemId = re.ItemId,
                                    ItemName = re.Item.ItemName,
                                    ItemTemp = re.Item.ItemTemp
                                },
                                Prep = re.Prep != null
                                    ? new PrepResponse
                                    {
                                        PrepId = re.Prep.PrepId,
                                        PrepName = re.Prep.PrepName
                                    }
                                    : null,
                                Amount = re.Amount
                            })
                            .OrderBy(re => re.RecipeSectionEntryIndex)
                            .ToList()
                    })
                    .OrderBy(rs => rs.RecipeSectionIndex)
                    .ToList()
            })
            .OrderBy(r => r.RecipeName)
            .ToList();
        
        return Ok(recipes);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecipeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public IActionResult Details([Required] Ulid recipeId)
    {
        if (!ModelState.IsValid && recipeId == Ulid.Empty)
        {
            return Error.BadRequestRecipeIdInvalid;
        }
        
        if (db.Recipes.Find(recipeId) == null)
        {
            return Error.NotFoundRecipe;
        }

        RecipeResponse recipe = db.Recipes
            .Select(r => new RecipeResponse
            {
                RecipeId = r.RecipeId,
                RecipeName = r.RecipeName,
                Url = r.Url,
                IsPinned = r.IsPinned,
                CartAmount = r.CartAmount,
                RecipeInstructionsResponse = r.RecipeInstructions
                    .Select(rs => new RecipeInstructionResponse
                    {
                        RecipeInstructionId = rs.RecipeInstructionId,
                        RecipeInstructionIndex = rs.RecipeInstructionIndex,
                        RecipeInstructionContent = rs.RecipeInstructionContent,
                        IsImage = rs.IsImage
                    })
                    .OrderBy(rs => rs.RecipeInstructionIndex)
                    .ToList(),
                RecipeSectionsResponse = r.RecipeSections
                    .Select(rs => new RecipeSectionResponse
                    {
                        RecipeSectionId = rs.RecipeSectionId,
                        RecipeSectionIndex = rs.RecipeSectionIndex,
                        RecipeSectionName = rs.RecipeSectionName,
                        RecipeSectionEntries = rs.RecipeSectionEntries
                            .Select(re => new RecipeSectionEntryResponse
                            {
                                RecipeSectionEntryId = re.RecipeSectionEntryId,
                                RecipeSectionEntryIndex = re.RecipeSectionEntryIndex,
                                Item = new ItemResponseNoPrep
                                {
                                    ItemId = re.ItemId,
                                    ItemName = re.Item.ItemName,
                                    ItemTemp = re.Item.ItemTemp
                                },
                                Prep = re.Prep != null
                                    ? new PrepResponse
                                    {
                                        PrepId = re.Prep.PrepId,
                                        PrepName = re.Prep.PrepName
                                    }
                                    : null,
                                Amount = re.Amount
                            })
                            .OrderBy(re => re.RecipeSectionEntryIndex)
                            .ToList()
                    })
                    .OrderBy(rs => rs.RecipeSectionIndex)
                    .ToList()
            })
            .First(s => s.RecipeId == recipeId);

        return Ok(recipe);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public IActionResult Add([Required] string recipeName)
    {
        if (!ModelState.IsValid || recipeName.Length == 0)
        {
            return Error.BadRequestRecipeNameInvalid;
        }
        
        db.Recipes.Add(new Recipe
        {
            RecipeName = recipeName
        });
        
        db.SaveChanges();
        return NoContent();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public IActionResult Edit([Required] Ulid recipeId, [Required] RecipeEditRequest recipeEditRequest)
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
            
                return Error.BadRequestRecipeEditRequestInvalid(errors);
            }
        }

        Recipe? recipe = db.Recipes.Find(recipeId);
        if (recipe == null)
        {
            return Error.NotFoundRecipe;
        }

        recipe.RecipeName = recipeEditRequest.RecipeName ?? recipe.RecipeName;
        recipe.Url = recipeEditRequest.Url ?? recipe.Url;
        recipe.IsPinned = recipeEditRequest.IsPinned ?? recipe.IsPinned;
        recipe.CartAmount = recipeEditRequest.CartAmount ?? recipe.CartAmount;

        db.SaveChanges();
        return NoContent();
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public IActionResult Delete([Required] Ulid recipeId)
    {
        if (!ModelState.IsValid && recipeId == Ulid.Empty)
        {
            return Error.BadRequestRecipeIdInvalid;
        }

        Recipe? recipe = db.Recipes.Find(recipeId);
        if (recipe == null)
        {
            return Error.NotFoundRecipe;
        }

        db.Recipes.Remove(recipe);
        db.SaveChanges();
        
        return NoContent();
    }
    
}