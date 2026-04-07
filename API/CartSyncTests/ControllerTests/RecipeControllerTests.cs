using System.Collections.Immutable;
using System.Net;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSync.Objects;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.Models.Seeding.SeedData;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class RecipeControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestRecipeAll()
    {
        ReadOnlyList<RecipeMinimalResponse> expected = SeedData.Recipes
            .AsQueryable()
            .OrderBy(recipe => recipe.RecipeName)
            .ThenBy(recipe => recipe.RecipeId)
            .Select(Recipe.ToMinimalResponse)
            .ToImmutableList()
            .WithValueSemantics();
        
        List<RecipeMinimalResponse> result = await RecipeController.All().ValueAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestRecipeDetails()
    {
        ReadOnlyList<RecipeResponse> expected = SeedData.Recipes
            .AsQueryable()
            .OrderBy(recipe => recipe.RecipeName)
            .ThenBy(recipe => recipe.RecipeId)
            .Select(Recipe.ToResponse)
            .ToImmutableList()
            .WithValueSemantics();
        
        foreach (RecipeResponse expectedRecipe in expected)
        {
            RecipeResponse result = await RecipeController.Details(expectedRecipe.Id).ValueAsync();
            Assert.Equal(expectedRecipe.ToMinimalResponse, result.ToMinimalResponse);
        }

        Ulid recipeId = SeedData.Recipes[1].RecipeId;
        
        ReadOnlyList<RecipeStepResponse> expectedSteps = SeedData.RecipeSteps
            .AsQueryable()
            .Where(recipe => recipe.RecipeId == recipeId)
            .OrderBy(recipe => recipe.SortOrder)
            .Select(RecipeStep.ToResponse)
            .ToImmutableList()
            .WithValueSemantics();
        
        RecipeResponse result2 = await RecipeController.Details(recipeId).ValueAsync();
        Assert.Equal(expectedSteps, result2.Steps);
        
        ReadOnlyList<RecipeSectionResponse> expectedSections = SeedData.RecipeSections
            .AsQueryable()
            .Where(section => section.RecipeId == recipeId)
            .OrderBy(section => section.SortOrder)
            .Select(RecipeSection.ToResponse)
            .ToImmutableList()
            .WithValueSemantics();
        
        Ulid? recipeSectionId = expectedSections[0].Id;
        ImmutableList<RecipeSectionEntryMinimal> expectedSectionEntries = SeedData.RecipeEntries
            .AsQueryable()
            .Where(entry => entry.RecipeSectionId == recipeSectionId)
            .OrderBy(entry => SeedData.Items.First(item => item.ItemId == entry.ItemId).Temp)
            .ThenBy(entry => SeedData.Items.First(item => item.ItemId == entry.ItemId).ItemName)
            .ThenBy(entry => entry.ItemId)
            .Select(entry => new RecipeSectionEntryMinimal(entry.RecipeEntryId, entry.ItemId, entry.PrepId))
            .ToImmutableList();

        ImmutableList<RecipeSectionEntryMinimal> actualSectionEntries = result2.Sections[0].Entries
            .Select(entry => new RecipeSectionEntryMinimal(entry.Id, entry.Item.Id, entry.Prep?.Id))
            .ToImmutableList();
        
        Assert.Single(result2.Sections);
        Assert.Equal(expectedSectionEntries, actualSectionEntries);
        
        foreach (RecipeSectionEntryMinimal entryMinimal in expectedSectionEntries)
        {
            string? itemName = SeedData.Items.Find(i => i.ItemId == entryMinimal.ItemId)?.ItemName;
            string? prepName = SeedData.Preps.Find(p => p.PrepId == entryMinimal.PrepId)?.PrepName;

            Assert.Contains(itemName,
                result2.Sections[0].Entries
                    .Where(entry => entry.Id == entryMinimal.EntryId)
                    .Select(entry => entry.Item.Name));
            
            Assert.Contains(prepName,
                result2.Sections[0].Entries
                    .Where(entry => entry.Id == entryMinimal.EntryId)
                    .Select(entry => entry.Prep?.Name));
        }
    }

    [Fact]
    public async Task TestRecipeAdd()
    {
        RecipeAddRequest recipeAddRequest = new()
        {
            Name = "New Recipe"
        };
        
        (RecipeResponse recipe, string location) result = await RecipeController.Add(recipeAddRequest).ValueAsync();
        Assert.Equal(recipeAddRequest.Name, result.recipe.Name);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.recipe.Id.ToString().ToLower());
        
        List<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(5, results.Count);
        Assert.Contains(recipeAddRequest.Name, results.Select(recipe => recipe.Name));
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
                    path = "/Name",
                    value = "New Recipe Name"
                }
            }
        };
        
        await RecipeController.Edit(recipeId, jsonPatch).AssertIsSuccessful();

        RecipeResponse result = await RecipeController.Details(recipeId).ValueAsync();
        Assert.Equal("New Recipe Name", result.Name);
        
        List<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.Contains("New Recipe Name", results.Select(recipe => recipe.Name));
        Assert.DoesNotContain(SeedData.Recipes[3].RecipeName, results.Select(recipe => recipe.Name));
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
        
        List<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.DoesNotContain(true, results.Select(recipe => recipe.IsPinned));
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

        List<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.DoesNotContain("New Recipe Name", results.Select(recipe => recipe.Name));
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

        List<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.DoesNotContain("New Recipe Name", results.Select(recipe => recipe.Name));
    }

    [Fact]
    public async Task TestRecipeDelete()
    {
        Ulid recipeId = SeedData.Recipes[3].RecipeId;
        Ulid recipeId2 = SeedData.Recipes[1].RecipeId;
        await RecipeController.Delete(recipeId).AssertIsSuccessful();
        
        Error error = await RecipeController.Details(recipeId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        List<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(3, results.Count);
        
        Error error2 = await RecipeController.Delete(recipeId).ErrorAsync();
        error2.AssertStatus(HttpStatusCode.NotFound);
        
        await RecipeController.Delete(recipeId2).AssertIsSuccessful();
        
        List<RecipeMinimalResponse> results2 = await RecipeController.All().ValueAsync();
        Assert.Equal(2, results2.Count);
    }

    [Fact]
    public async Task TestRecipeDelete_RecipeNotFound()
    {
        Ulid recipeId = Ulid.NotFound;
        Error error = await RecipeController.Delete(recipeId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        List<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
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