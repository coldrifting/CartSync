using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Recipes")]
[Route("/api/recipes")]
public class RecipeController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetRecipe(Ulid? recipeId)
    {
        if (recipeId == null)
        {
            List<RecipeResponse> recipes = db.Recipes
                .Select(r => new RecipeResponse
                {
                    RecipeId = r.RecipeId,
                    RecipeName = r.RecipeName,
                    Url = r.Url,
                    IsPinned = r.IsPinned,
                    CartAmount = r.CartAmount,
                    RecipeStepsResponse = r.RecipeSteps
                        .Select(rs => new RecipeStepResponse
                        {
                            RecipeStepId = rs.RecipeStepId,
                            RecipeStepOrder = rs.RecipeStepOrder,
                            RecipeStepContent = rs.RecipeStepContent,
                            IsImage = rs.IsImage
                        })
                        .ToList(),
                    RecipeSectionsResponse = r.RecipeSections
                        .Select(rs => new RecipeSectionResponse
                        {
                            RecipeSectionId = rs.RecipeSectionId,
                            RecipeSectionOrder = rs.RecipeSectionOrder,
                            RecipeSectionName = rs.RecipeSectionName,
                            RecipeSectionEntries = rs.RecipeSectionEntries
                                .Select(re => new RecipeSectionEntryResponse
                                {
                                    RecipeSectionEntryId = re.RecipeSectionEntryId,
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
                                .ToList()
                        }).ToList()
                })
                .ToList();
            
            return Ok(recipes);
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
                RecipeStepsResponse = r.RecipeSteps
                    .Select(rs => new RecipeStepResponse
                    {
                        RecipeStepId = rs.RecipeStepId,
                        RecipeStepOrder = rs.RecipeStepOrder,
                        RecipeStepContent = rs.RecipeStepContent,
                        IsImage = rs.IsImage
                    })
                    .ToList(),
                RecipeSectionsResponse = r.RecipeSections
                    .Select(rs => new RecipeSectionResponse
                    {
                        RecipeSectionId = rs.RecipeSectionId,
                        RecipeSectionOrder = rs.RecipeSectionOrder,
                        RecipeSectionName = rs.RecipeSectionName,
                        RecipeSectionEntries = rs.RecipeSectionEntries
                            .Select(re => new RecipeSectionEntryResponse
                            {
                                RecipeSectionEntryId = re.RecipeSectionEntryId,
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
                            .ToList()
                    }).ToList()
            })
            .First(s => s.RecipeId == recipeId);

        return Ok(recipe);
    }
}