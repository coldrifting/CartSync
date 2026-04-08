using System.Net;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Objects;
using CartSync.SeedData;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Microsoft.EntityFrameworkCore;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class RecipeEntryControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestAdd_NoPrep()
    {
        Ulid recipeId = SeedData.RecipeSections[0].RecipeId;
        Ulid sectionId = SeedData.RecipeSections[0].RecipeSectionId;

        RecipeEntryAddRequest recipeEntryAddRequest = new()
        {
            Amount = Amount.VolumeCups(3),
            ItemId = SeedData.Items[35].ItemId,
            PrepId = null
        };
        
        (RecipeEntryResponse response, string location) result = await RecipeEntryController.Add(sectionId, recipeEntryAddRequest).ValueAsync();
        Assert.NotNull(result.location);
        Assert.NotNull(result.response);

        RecipeEntry? entry = await Context.RecipeEntries
            .Include(r => r.RecipeSection)
            .FirstOrDefaultAsync(r => r.RecipeEntryId == Ulid.Parse(result.location.Split('/').Last()), TestContext.Current.CancellationToken);
        
        Assert.NotNull(entry);
        Assert.Equal(sectionId, entry.RecipeSectionId);
        Assert.Equal(recipeId, entry.RecipeSection.RecipeId);
        Assert.Equal(recipeEntryAddRequest.Amount, entry.Amount);
        Assert.Equal(recipeEntryAddRequest.ItemId, entry.ItemId);
        Assert.Null(recipeEntryAddRequest.PrepId);
    }
    
    [Fact]
    public async Task TestAdd_WithPrep()
    {
        Ulid recipeId = SeedData.RecipeSections[0].RecipeId;
        Ulid sectionId = SeedData.RecipeSections[0].RecipeSectionId;

        RecipeEntryAddRequest recipeEntryAddRequest = new()
        {
            Amount = Amount.VolumeCups(3),
            ItemId = SeedData.Items[182].ItemId,
            PrepId = SeedData.Preps[4].PrepId
        };
        
        (RecipeEntryResponse response, string location) result = await RecipeEntryController.Add(sectionId, recipeEntryAddRequest).ValueAsync();
        Assert.NotNull(result.location);
        Assert.NotNull(result.response);

        RecipeEntry? entry = await Context.RecipeEntries
            .Include(r => r.RecipeSection)
            .FirstOrDefaultAsync(r => r.RecipeEntryId == Ulid.Parse(result.location.Split('/').Last()), TestContext.Current.CancellationToken);
        
        Assert.NotNull(entry);
        Assert.Equal(sectionId, entry.RecipeSectionId);
        Assert.Equal(recipeId, entry.RecipeSection.RecipeId);
        Assert.Equal(recipeEntryAddRequest.Amount, entry.Amount);
        Assert.Equal(recipeEntryAddRequest.ItemId, entry.ItemId);
        Assert.Equal(recipeEntryAddRequest.PrepId, entry.PrepId);
    }
    
    [Fact]
    public async Task TestAdd_DuplicateItem_DiffPrep()
    {
        Ulid recipeId = SeedData.RecipeSections[1].RecipeId;
        Ulid sectionId = SeedData.RecipeSections[1].RecipeSectionId;

        RecipeEntryAddRequest recipeEntryAddRequest = new()
        {
            Amount = Amount.VolumeCups(3),
            ItemId = SeedData.Items[180].ItemId,
            PrepId = SeedData.Preps[4].PrepId
        };
        
        (RecipeEntryResponse response, string location) result = await RecipeEntryController.Add(sectionId, recipeEntryAddRequest).ValueAsync();
        Assert.NotNull(result.location);
        Assert.NotNull(result.response);

        RecipeEntry? entry = await Context.RecipeEntries
            .Include(r => r.RecipeSection)
            .FirstOrDefaultAsync(r => r.RecipeEntryId == Ulid.Parse(result.location.Split('/').Last()), TestContext.Current.CancellationToken);
        
        Assert.NotNull(entry);
        Assert.Equal(sectionId, entry.RecipeSectionId);
        Assert.Equal(recipeId, entry.RecipeSection.RecipeId);
        Assert.Equal(recipeEntryAddRequest.Amount, entry.Amount);
        Assert.Equal(recipeEntryAddRequest.ItemId, entry.ItemId);
        Assert.Equal(recipeEntryAddRequest.PrepId, entry.PrepId);
    }
    
    [Fact]
    public async Task TestAdd_DuplicateItem_SamePrep_ShouldError()
    {
        Ulid sectionId = SeedData.RecipeSections[1].RecipeSectionId;

        RecipeEntryAddRequest recipeEntryAddRequest = new()
        {
            Amount = Amount.VolumeCups(3),
            ItemId = SeedData.Items[180].ItemId,
            PrepId = SeedData.Preps[3].PrepId
        };
        
        ErrorResponse errorResponse = await RecipeEntryController.Add(sectionId, recipeEntryAddRequest).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.Conflict);

        RecipeEntry entry = await Context.RecipeEntries
            .SingleAsync(rse => 
                rse.RecipeSectionId == sectionId &&
                rse.ItemId == recipeEntryAddRequest.ItemId &&
                rse.PrepId == recipeEntryAddRequest.PrepId, TestContext.Current.CancellationToken);
        
        Assert.NotEqual(recipeEntryAddRequest.Amount, entry.Amount);
        Assert.Equal(recipeEntryAddRequest.ItemId, entry.ItemId);
        Assert.Equal(recipeEntryAddRequest.PrepId, entry.PrepId);
    }
    
    [Fact]
    public async Task TestAdd_WithInvalidPrep_ShouldError()
    {
        Ulid sectionId = SeedData.RecipeSections[0].RecipeSectionId;

        RecipeEntryAddRequest recipeEntryAddRequest = new()
        {
            Amount = Amount.VolumeCups(3),
            ItemId = SeedData.Items[182].ItemId,
            PrepId = SeedData.Preps[0].PrepId
        };
        
        ErrorResponse errorResponse = await RecipeEntryController.Add(sectionId, recipeEntryAddRequest).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);

        RecipeEntry? entry = await Context.RecipeEntries
            .FirstOrDefaultAsync(rse => rse.ItemId == recipeEntryAddRequest.ItemId &&
                                        rse.PrepId == recipeEntryAddRequest.PrepId, TestContext.Current.CancellationToken);
        
        Assert.Null(entry);
    }
    
    [Fact]
    public async Task TestEdit_RemovePrep()
    {
        Ulid sectionId = SeedData.RecipeSections[0].RecipeSectionId;
        Ulid recipeEntryId = SeedData.RecipeEntries[3].RecipeEntryId;
        Ulid itemId = SeedData.Items[207].ItemId;

        JsonPatchDocument<RecipeEntryEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<RecipeEntryEditRequest>
                {
                    op = "replace",
                    path = "/PrepId",
                    value = null
                }
            }
        };
        
        await RecipeEntryController.Edit(recipeEntryId, jsonPatch).AssertIsSuccessful();

        RecipeEntry? entry = await Context.RecipeEntries.FindAsync([recipeEntryId], TestContext.Current.CancellationToken);
        
        Assert.NotNull(entry);
        Assert.Equal(sectionId, entry.RecipeSectionId);
        Assert.Equal(recipeEntryId, entry.RecipeEntryId);
        Assert.Equal(itemId, entry.ItemId);
        Assert.Null(entry.PrepId);
    }
    
    [Fact]
    public async Task TestEdit_AddPrep()
    {
        Ulid sectionId = SeedData.RecipeSections[0].RecipeSectionId;
        Ulid itemId = SeedData.Items[184].ItemId;
        Ulid prepId = SeedData.Preps[4].PrepId;

        JsonPatchDocument<RecipeEntryEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<RecipeEntryEditRequest>
                {
                    op = "replace",
                    path = "/PrepId",
                    value = prepId
                }
            }
        };
        
        Ulid recipeEntryId = Ulid.Parse((await RecipeEntryController.Add(sectionId, new RecipeEntryAddRequest
        {
            Amount = Amount.VolumeCups(3),
            ItemId = itemId,
            PrepId = null
        }).ValueAsync()).Item2.Split('/').Last());
        
        await RecipeEntryController.Edit(recipeEntryId, jsonPatch).AssertIsSuccessful();

        RecipeEntry? entry = await Context.RecipeEntries.FindAsync([recipeEntryId], TestContext.Current.CancellationToken);
        
        Assert.NotNull(entry);
        Assert.Equal(sectionId, entry.RecipeSectionId);
        Assert.Equal(recipeEntryId, entry.RecipeEntryId);
        Assert.Equal(itemId, entry.ItemId);
        Assert.Equal(prepId, entry.PrepId);
    }
    
    [Fact]
    public async Task TestEdit_RemovePrep_NoPrepAlreadyExists_ShouldError()
    {
        Ulid sectionId = SeedData.RecipeSections[0].RecipeSectionId;
        Ulid recipeEntryId = SeedData.RecipeEntries[3].RecipeEntryId;
        Ulid itemId = SeedData.Items[207].ItemId;

        JsonPatchDocument<RecipeEntryEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<RecipeEntryEditRequest>
                {
                    op = "replace",
                    path = "/PrepId",
                    value = null
                }
            }
        };
        
        await RecipeEntryController.Add(sectionId, new RecipeEntryAddRequest
        {
            Amount = Amount.VolumeCups(3),
            ItemId = itemId,
            PrepId = null
        });
        
        ErrorResponse errorResponse = await RecipeEntryController.Edit(recipeEntryId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.Conflict);
        
        RecipeEntry entry = await Context.RecipeEntries
            .SingleAsync(r => r.RecipeSectionId == sectionId && r.ItemId == itemId && r.PrepId == null, TestContext.Current.CancellationToken);
        
        Assert.Equal(sectionId, entry.RecipeSectionId);
        Assert.Equal(itemId, entry.ItemId);
        Assert.Null(entry.PrepId);
    }
}