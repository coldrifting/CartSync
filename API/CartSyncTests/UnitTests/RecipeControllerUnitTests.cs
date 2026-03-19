using System.Collections.Immutable;
using System.Net;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Objects;
using CartSyncTests.Core;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.Models.Seeding.SeedData;

namespace CartSyncTests.UnitTests;

[Collection("DatabaseTests")]
public class RecipeControllerUnitTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestRecipeAll()
    {
        ReadOnlyList<RecipeResponse> expected = SeedData.Recipes
            .AsQueryable()
            .OrderBy(r => r.RecipeName)
            .ThenBy(r => r.RecipeId)
            .Select(Recipe.ToResponse)
            .ToImmutableList()
            .WithValueSemantics();
        
        List<RecipeResponse> result = await RecipeController.All().ValueAsync();
        Assert.Equal(expected.Select(s => s.ToMinimalResponse), result.Select(s => s.ToMinimalResponse));
    }

    [Fact]
    public async Task TestRecipeDetails()
    {
        ReadOnlyList<RecipeResponse> expected = SeedData.Recipes
            .AsQueryable()
            .OrderBy(r => r.RecipeName)
            .ThenBy(r => r.RecipeId)
            .Select(Recipe.ToResponse)
            .ToImmutableList()
            .WithValueSemantics();
        
        foreach (RecipeResponse expectedRecipe in expected)
        {
            RecipeResponse result = await RecipeController.Details(expectedRecipe.RecipeId).ValueAsync();
            Assert.Equal(expectedRecipe.ToMinimalResponse, result.ToMinimalResponse);
        }

        Ulid recipeId = SeedData.Recipes[1].RecipeId;
        
        ReadOnlyList<RecipeInstructionResponse> expectedInstructions = SeedData.RecipeInstructions
            .AsQueryable()
            .Where(r => r.RecipeId == recipeId)
            .OrderBy(r => r.SortOrder)
            .Select(RecipeInstruction.ToResponse)
            .ToImmutableList()
            .WithValueSemantics();
        
        RecipeResponse result2 = await RecipeController.Details(recipeId).ValueAsync();
        Assert.Equal(expectedInstructions, result2.RecipeInstructionsResponse);
        
        ReadOnlyList<RecipeSectionResponse> expectedSections = SeedData.RecipeSections
            .AsQueryable()
            .Where(r => r.RecipeId == recipeId)
            .OrderBy(r => r.SortOrder)
            .Select(RecipeSection.ToResponse)
            .ToImmutableList()
            .WithValueSemantics();
        
        Ulid? recipeSectionId = expectedSections[0].RecipeSectionId;
        ImmutableList<RecipeSectionEntryMinimal> expectedSectionEntries = SeedData.RecipeSectionEntries
            .AsQueryable()
            .Where(r => r.RecipeSectionId == recipeSectionId)
            .OrderBy(r => r.SortOrder)
            .Select(r => new RecipeSectionEntryMinimal(r.RecipeSectionEntryId, r.ItemId, r.PrepId))
            .ToImmutableList();

        ImmutableList<RecipeSectionEntryMinimal> actualSectionEntries = result2.RecipeSectionsResponse[0].RecipeSectionEntries
            .Select(r => new RecipeSectionEntryMinimal(r.RecipeSectionEntryId, r.Item.ItemId, r.Prep?.PrepId))
            .ToImmutableList();
        
        Assert.Single(result2.RecipeSectionsResponse);
        Assert.Equal(expectedSectionEntries, actualSectionEntries);
        
        foreach (RecipeSectionEntryMinimal entry in expectedSectionEntries)
        {
            string? itemName = SeedData.Items.Find(i => i.ItemId == entry.ItemId)?.ItemName;
            string? prepName = SeedData.Preps.Find(p => p.PrepId == entry.PrepId)?.PrepName;

            Assert.Contains(itemName,
                result2.RecipeSectionsResponse[0].RecipeSectionEntries
                    .Where(r => r.RecipeSectionEntryId == entry.EntryId)
                    .Select(r => r.Item.ItemName));
            
            Assert.Contains(prepName,
                result2.RecipeSectionsResponse[0].RecipeSectionEntries
                    .Where(r => r.RecipeSectionEntryId == entry.EntryId)
                    .Select(r => r.Prep?.PrepName));
        }
    }

    [Fact]
    public async Task TestRecipeAdd()
    {
        RecipeAddRequest recipeAddRequest = new()
        {
            RecipeName = "New Recipe"
        };
        
        (RecipeResponse recipe, string location) result = await RecipeController.Add(recipeAddRequest).ValueAsync();
        Assert.Equal(recipeAddRequest.RecipeName, result.recipe.RecipeName);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.recipe.RecipeId.ToString().ToLower());
        
        List<RecipeResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(5, results.Count);
        Assert.Contains(recipeAddRequest.RecipeName, results.Select(r => r.RecipeName));
    }

    [Fact]
    public async Task TestRecipeEdit_Rename()
    {
        Ulid recipeId = SeedData.Recipes[3].RecipeId;
        JsonPatchDocument<RecipeEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<RecipeEditRequest>
                {
                    op = "replace",
                    path = "/RecipeName",
                    value = "New Recipe Name"
                }
            }
        };
        
        await RecipeController.Edit(recipeId, jsonPatch).AssertIsSuccessful();

        RecipeResponse result = await RecipeController.Details(recipeId).ValueAsync();
        Assert.Equal("New Recipe Name", result.RecipeName);
        
        List<RecipeResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.Contains("New Recipe Name", results.Select(r => r.RecipeName));
        Assert.DoesNotContain(SeedData.Recipes[3].RecipeName, results.Select(r => r.RecipeName));
    }

    [Fact]
    public async Task TestRecipeEdit_UrlAndIsPinned()
    {
        Ulid recipeId = SeedData.Recipes[1].RecipeId;
        JsonPatchDocument<RecipeEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<RecipeEditRequest>
                {
                    op = "replace",
                    path = "/Url",
                    value = "https://cartsync.app"
                },
                new Operation<RecipeEditRequest>
                {
                    op = "replace",
                    path = "/IsPinned",
                    value = false
                }
            }
        };
        
        await RecipeController.Edit(recipeId, jsonPatch).AssertIsSuccessful();

        RecipeResponse result = await RecipeController.Details(recipeId).ValueAsync();
        Assert.Equal("https://cartsync.app", result.Url);
        Assert.False(result.IsPinned);
        
        List<RecipeResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.Contains("https://cartsync.app", results.Select(r => r.Url));
        Assert.DoesNotContain(SeedData.Recipes[1].Url, results.Select(r => r.Url));
        Assert.DoesNotContain(true, results.Select(r => r.IsPinned));
    }

    [Fact]
    public async Task TestRecipeEdit_InvalidRecipeId()
    {
        Ulid recipeId = Ulid.NotFound;
        JsonPatchDocument<RecipeEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<RecipeEditRequest>
                {
                    op = "replace",
                    path = "/RecipeName",
                    value = "New Recipe Name"
                }
            }
        };
        
        Error error = await RecipeController.Edit(recipeId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);

        List<RecipeResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.DoesNotContain("New Recipe Name", results.Select(r => r.RecipeName));
    }

    [Fact]
    public async Task TestRecipeEdit_InvalidData()
    {
        Ulid recipeId = SeedData.Recipes[1].RecipeId;
        JsonPatchDocument<RecipeEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<RecipeEditRequest>
                {
                    op = "remove",
                    path = "/RecipeName"
                }
            }
        };
        
        Error error = await RecipeController.Edit(recipeId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);

        List<RecipeResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.DoesNotContain("New Recipe Name", results.Select(r => r.RecipeName));
    }

    [Fact]
    public async Task TestRecipeDelete()
    {
        Ulid recipeId = SeedData.Recipes[3].RecipeId;
        Ulid recipeId2 = SeedData.Recipes[1].RecipeId;
        await RecipeController.Delete(recipeId).AssertIsSuccessful();
        
        Error error = await RecipeController.Details(recipeId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        List<RecipeResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(3, results.Count);
        
        Error error2 = await RecipeController.Delete(recipeId).ErrorAsync();
        error2.AssertStatus(HttpStatusCode.NotFound);
        
        await RecipeController.Delete(recipeId2).AssertIsSuccessful();
        
        List<RecipeResponse> results2 = await RecipeController.All().ValueAsync();
        Assert.Equal(2, results2.Count);
    }

    [Fact]
    public async Task TestRecipeDelete_RecipeNotFound()
    {
        Ulid recipeId = Ulid.NotFound;
        Error error = await RecipeController.Delete(recipeId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        List<RecipeResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
    }

    private record RecipeSectionEntryMinimal(Ulid EntryId, Ulid? ItemId, Ulid? PrepId)
    {
        public override string ToString()
        {
            return $"{{ id = {EntryId}, item = {ItemId}, prep = {PrepId} }}";
        }
    }
}