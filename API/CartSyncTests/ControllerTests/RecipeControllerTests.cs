using System.Collections.Immutable;
using System.Net;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Objects;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.SeedData.SeedData;

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
            .Select(RecipeMinimalResponse.FromEntity)
            .ToReadOnlyList();
        
        ReadOnlyList<RecipeMinimalResponse> result = await RecipeController.All().ValueAsync();
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task TestRecipeDetails()
    {
        ReadOnlyList<RecipeResponse> expected = SeedData.Recipes
            .OrderBy(recipe => recipe.RecipeName)
            .ThenBy(recipe => recipe.RecipeId)
            .Select(recipe => new RecipeResponse
            {
                Id = recipe.RecipeId,
                Name = recipe.RecipeName,
                Url = recipe.Url,
                IsPinned = recipe.IsPinned,
                Steps = SeedData.RecipeSteps
                    .Where(step => step.RecipeId == recipe.RecipeId)
                    .AsQueryable()
                    .Select(RecipeStepResponse.FromEntity)
                    .OrderBy(step => step.SortOrder)
                    .ToReadOnlyList(),
                Sections = SeedData.RecipeSections
                    .Where(section => section.RecipeId == recipe.RecipeId)
                    .Select(section => new RecipeSectionResponse
                    {
                        Id = section.RecipeSectionId,
                        Name = section.RecipeSectionName,
                        SortOrder = section.SortOrder,
                        Entries = SeedData.RecipeEntries
                            .Where(entry => entry.RecipeSectionId == section.RecipeSectionId)
                            .Select(entry => new RecipeEntryResponse
                            {
                                Id = entry.RecipeEntryId,
                                Amount = entry.Amount,
                                Item = SeedData.Items
                                    .Where(item => item.ItemId == entry.ItemId)
                                    .AsQueryable()
                                    .Select(ItemMinimalResponse.FromEntity)
                                    .First(),
                                Prep = SeedData.Preps
                                    .Where(prep => prep.PrepId == entry.PrepId)
                                    .AsQueryable()
                                    .Select(PrepResponse.FromEntity)
                                    .FirstOrDefault()
                            })
                            .OrderBy(entry => entry.Item.Temp)
                            .ThenBy(entry => entry.Item.Name)
                            .ThenBy(entry => entry.Item.Id)
                            .ThenBy(entry => entry.Prep != null ? entry.Prep.Name : "$None")
                            .ToReadOnlyList()
                    })
                    .OrderBy(section => section.SortOrder)
                    .ToReadOnlyList()
            })
            .ToReadOnlyList();
        
        foreach (RecipeResponse expectedRecipe in expected)
        {
            RecipeResponse actualRecipe = await RecipeController.Details(expectedRecipe.Id).ValueAsync();
            Assert.Equal(expectedRecipe, actualRecipe);
        }

        Ulid recipeId = SeedData.Recipes[1].RecipeId;

        ReadOnlyList<RecipeStepResponse> expectedSteps = SeedData.RecipeSteps
            .AsQueryable()
            .Where(recipe => recipe.RecipeId == recipeId)
            .OrderBy(recipe => recipe.SortOrder)
            .Select(RecipeStepResponse.FromEntity)
            .ToReadOnlyList();
        
        RecipeResponse result2 = await RecipeController.Details(recipeId).ValueAsync();
        Assert.Equal(expectedSteps, result2.Steps);

        ReadOnlyList<RecipeSectionResponse> expectedSections = SeedData.RecipeSections
            .AsQueryable()
            .Where(section => section.RecipeId == recipeId)
            .OrderBy(section => section.SortOrder)
            .Select(RecipeSectionResponse.FromEntity)
            .ToReadOnlyList();
        
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
        AddRequest recipeAddRequest = new()
        {
            Name = "New Recipe"
        };
        
        (RecipeMinimalResponse recipe, string location) result = await RecipeController.Add(recipeAddRequest).ValueAsync();
        Assert.Equal(recipeAddRequest.Name, result.recipe.Name);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.recipe.Id.ToString().ToLower());
        
        ReadOnlyList<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
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
        
        ReadOnlyList<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
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
        
        ReadOnlyList<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
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
        
        ErrorResponse errorResponse = await RecipeController.Edit(recipeId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);

        ReadOnlyList<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
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
        
        ErrorResponse errorResponse = await RecipeController.Edit(recipeId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.BadRequest);

        ReadOnlyList<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(4, results.Count);
        Assert.DoesNotContain("New Recipe Name", results.Select(recipe => recipe.Name));
    }

    [Fact]
    public async Task TestRecipeDelete()
    {
        Ulid recipeId = SeedData.Recipes[3].RecipeId;
        Ulid recipeId2 = SeedData.Recipes[1].RecipeId;
        await RecipeController.Delete(recipeId).AssertIsSuccessful();
        
        ErrorResponse errorResponse = await RecipeController.Details(recipeId).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        ReadOnlyList<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
        Assert.Equal(3, results.Count);
        
        ErrorResponse error2 = await RecipeController.Delete(recipeId).ErrorAsync();
        error2.AssertStatus(HttpStatusCode.NotFound);
        
        await RecipeController.Delete(recipeId2).AssertIsSuccessful();
        
        ReadOnlyList<RecipeMinimalResponse> results2 = await RecipeController.All().ValueAsync();
        Assert.Equal(2, results2.Count);
    }

    [Fact]
    public async Task TestRecipeDelete_RecipeNotFound()
    {
        Ulid recipeId = Ulid.NotFound;
        ErrorResponse errorResponse = await RecipeController.Delete(recipeId).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        ReadOnlyList<RecipeMinimalResponse> results = await RecipeController.All().ValueAsync();
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