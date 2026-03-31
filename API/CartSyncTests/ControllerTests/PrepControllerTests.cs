using System.Net;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.Models.Seeding.SeedData;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class PrepControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestPrepAll()
    {
        List<PrepResponse> expectedPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();

        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(expectedPreps, preps);
    }
    
    [Fact]
    public async Task TestPrepUsages_ItemAndRecipeUsagesFound()
    {
        PrepUsagesResponse expected = new()
        {
            PrepId = SeedData.Preps[3].PrepId,
            PrepName = SeedData.Preps[3].PrepName,
            Items = [
                new ItemMinimalResponse
                {
                    ItemId = SeedData.Items[179].ItemId,
                    ItemName = SeedData.Items[179].ItemName,
                    ItemTemp = SeedData.Items[179].ItemTemp,
                },
                new ItemMinimalResponse
                {
                    ItemId = SeedData.Items[180].ItemId,
                    ItemName = SeedData.Items[180].ItemName,
                    ItemTemp = SeedData.Items[180].ItemTemp,
                },
                new ItemMinimalResponse
                {
                    ItemId = SeedData.Items[181].ItemId,
                    ItemName = SeedData.Items[181].ItemName,
                    ItemTemp = SeedData.Items[181].ItemTemp,
                },
                new ItemMinimalResponse
                {
                    ItemId = SeedData.Items[182].ItemId,
                    ItemName = SeedData.Items[182].ItemName,
                    ItemTemp = SeedData.Items[182].ItemTemp,
                },
                new ItemMinimalResponse
                {
                    ItemId = SeedData.Items[183].ItemId,
                    ItemName = SeedData.Items[183].ItemName,
                    ItemTemp = SeedData.Items[183].ItemTemp,
                },
                new ItemMinimalResponse
                {
                    ItemId = SeedData.Items[184].ItemId,
                    ItemName = SeedData.Items[184].ItemName,
                    ItemTemp = SeedData.Items[184].ItemTemp,
                }
            ],
            Recipes = [
                new RecipeMinimalResponse
                {
                    RecipeId = SeedData.Recipes[1].RecipeId,
                    RecipeName = SeedData.Recipes[1].RecipeName,
                    IsPinned =  SeedData.Recipes[1].IsPinned,
                },
                new RecipeMinimalResponse
                {
                    RecipeId = SeedData.Recipes[3].RecipeId,
                    RecipeName = SeedData.Recipes[3].RecipeName,
                    IsPinned =  SeedData.Recipes[3].IsPinned,
                }
            ]
        };
        
        PrepUsagesResponse result = await PrepController.Usages(SeedData.Preps[3].PrepId).ValueAsync();
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public async Task TestPrepUsages_OnlyItemUsagesFound()
    {
        PrepUsagesResponse expected = new()
        {
            PrepId = SeedData.Preps[6].PrepId,
            PrepName = SeedData.Preps[6].PrepName,
            Items =
            [
                new ItemMinimalResponse
                {
                    ItemId = SeedData.Items[56].ItemId,
                    ItemName = SeedData.Items[56].ItemName,
                    ItemTemp = SeedData.Items[56].ItemTemp,
                },
            ],
            Recipes = []
        };
        
        PrepUsagesResponse result = await PrepController.Usages(SeedData.Preps[6].PrepId).ValueAsync();
        Assert.Equal(expected, result);
    }
    
    [Fact]
    public async Task TestPrepUsages_PrepNotFound()
    {
        Error error = await PrepController.Usages(Ulid.NotFound).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task TestPrepAdd()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();

        PrepAddRequest addRequest = new()
        {
            PrepName = "New Prep Name"
        };
        (PrepResponse prep, string location) result = await PrepController.Add(addRequest).ValueAsync();
        Assert.Equal(addRequest.PrepName, result.prep.PrepName);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.prep.PrepId.ToString().ToLower());
        
        List<PrepResponse> preps = await PrepController.All().ValueAsync();
        
        Assert.Equal(previousPreps.Count + 1, preps.Count);
        Assert.Contains(preps, p => p.PrepName == addRequest.PrepName);
    }

    [Fact]
    public async Task TestPrepEdit_Rename()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();
        
        JsonPatchDocument<PrepEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<PrepEditRequest>
                {
                    op = "replace",
                    path = "/PrepName",
                    value = "New Prep Name"
                }
            }
        };
        
        Ulid prepId = SeedData.Preps[3].PrepId;
        await PrepController.Edit(prepId, jsonPatch).AssertIsSuccessful();
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId && p.PrepName == "New Prep Name");
    }

    [Fact]
    public async Task TestPrepEdit_BadPrepId()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();
        
        JsonPatchDocument<PrepEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<PrepEditRequest>
                {
                    op = "replace",
                    path = "/PrepName",
                    value = "New Prep Name"
                }
            }
        };
        
        Ulid prepId = SeedData.Preps[3].PrepId;
        Error error = await PrepController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId && p.PrepName != "New Prep Name");
    }

    [Fact]
    public async Task TestPrepEdit_ReplacePrepIdWithBadIdShouldError()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();
        
        Ulid badPrepId = SeedData.Preps[5].PrepId;
        JsonPatchDocument<PrepEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<PrepEditRequest>
                {
                    op = "replace",
                    path = "/PrepId",
                    value = $"{badPrepId}"
                }
            }
        };
        
        Ulid prepId = SeedData.Preps[3].PrepId;
        Error error = await PrepController.Edit(prepId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId);
        Assert.Single(preps, p => p.PrepId == badPrepId);
    }

    [Fact]
    public async Task TestPrepEdit_ReplacePrepIdWithNotFoundIdShouldError()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();
        
        JsonPatchDocument<PrepEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<PrepEditRequest>
                {
                    op = "replace",
                    path = "/PrepId",
                    value = $"{Ulid.NotFound}"
                }
            }
        };
        
        Ulid prepId = SeedData.Preps[3].PrepId;
        Error error = await PrepController.Edit(prepId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId);
        Assert.DoesNotContain(preps, p => p.PrepId == Ulid.NotFound);
    }

    [Fact]
    public async Task TestPrepEdit_RemoveNameShouldError()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();
        
        JsonPatchDocument<PrepEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<PrepEditRequest>
                {
                    op = "remove",
                    path = "/PrepName"
                }
            }
        };
        
        Ulid prepId = SeedData.Preps[3].PrepId;
        Error error = await PrepController.Edit(prepId, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.BadRequest);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId && p.PrepName != "New Prep Name");
    }

    [Fact]
    public async Task TestPrepDelete_Cascade()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();

        await PrepController.Delete(SeedData.Preps[3].PrepId).AssertIsSuccessful();
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count - 1, preps.Count);
        Assert.DoesNotContain(preps, p => Equals(p, Prep.ToResponse.Compile()(SeedData.Preps[3])));
    }
    
    [Fact]
    public async Task TestPrepDelete_NoForeignKeys()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();

        PrepAddRequest addRequest = new()
        {
            PrepName = "New Prep Name"
        };
        (PrepResponse prep, string location) result = await PrepController.Add(addRequest).ValueAsync();
        Assert.Equal(addRequest.PrepName, result.prep.PrepName);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.prep.PrepId.ToString().ToLower());
        
        List<PrepResponse> prepsAfterAdd = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count + 1, prepsAfterAdd.Count);
        Assert.Contains(prepsAfterAdd, p => p.PrepName == addRequest.PrepName);
        
        Ulid prepId = prepsAfterAdd.Single(p => p.PrepName == addRequest.PrepName).PrepId;

        await PrepController.Delete(prepId).AssertIsSuccessful();
        
        Error error = await PrepController.Delete(prepId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.DoesNotContain(preps, p => p.PrepName == addRequest.PrepName);
    }
    
    [Fact]
    public async Task TestPrepDelete_PrepNotFound()
    {
        Error error = await PrepController.Delete(Ulid.NotFound).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }
}