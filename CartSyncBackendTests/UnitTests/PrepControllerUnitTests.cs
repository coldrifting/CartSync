using System.Net;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Database.Seeding;
using CartSyncBackendTests.Core;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Microsoft.AspNetCore.Mvc;

namespace CartSyncBackendTests.UnitTests;

[Collection("DatabaseTests")]
public class PrepControllerUnitTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestGetPreps()
    {
        List<PrepResponse> expectedPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();

        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(expectedPreps, preps);
    }
    
    [Fact]
    public async Task TestGetPrepUsages()
    {
        UsageResponse expected = new()
        {
            {
                "Items", 
                [
                    (SeedData.Items[179].ItemId,  SeedData.Items[179].ItemName),
                    (SeedData.Items[180].ItemId,  SeedData.Items[180].ItemName),
                    (SeedData.Items[181].ItemId,  SeedData.Items[181].ItemName),
                    (SeedData.Items[182].ItemId,  SeedData.Items[182].ItemName),
                    (SeedData.Items[183].ItemId,  SeedData.Items[183].ItemName),
                    (SeedData.Items[184].ItemId,  SeedData.Items[184].ItemName),
                ]
            },
            {
                "Recipes", 
                [
                    (SeedData.Recipes[1].RecipeId,  SeedData.Recipes[1].RecipeName),
                    (SeedData.Recipes[3].RecipeId,  SeedData.Recipes[3].RecipeName),
                ]
            },
        };
        
        IActionResult result = await PrepController.Usages(SeedData.Preps[3].PrepId);
        Assert.IsType<OkObjectResult>(result, exactMatch: false);

        if (result is not OkObjectResult resultData)
        {
            Assert.Fail();
            return;
        }

        Assert.Equal(expected, resultData.Value, Extensions.UsageResponseComparer);
    }
    
    [Fact]
    public async Task TestGetPrepUsagesOnlyItemUses()
    {
        UsageResponse expected = new()
        {
            {
                "Items", 
                [
                    (SeedData.Items[56].ItemId,  SeedData.Items[56].ItemName)
                ]
            }
        };
        
        IActionResult result = await PrepController.Usages(SeedData.Preps[6].PrepId);
        Assert.IsType<OkObjectResult>(result, exactMatch: false);

        if (result is not OkObjectResult resultData)
        {
            Assert.Fail();
            return;
        }

        Assert.Equal(expected, resultData.Value, Extensions.UsageResponseComparer);
    }
    
    [Fact]
    public async Task TestGetPrepUsagesPrepNotFound()
    {
        Error result = await PrepController.Usages(Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }
    
    [Fact]
    public async Task TestAddPrep()
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
        IActionResult result = await PrepController.Add(addRequest);
        Assert.IsType<NoContentResult>(result, exactMatch: false);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count + 1, preps.Count);
        Assert.Contains(preps, p => p.PrepName == addRequest.PrepName);
    }

    [Fact]
    public async Task TestPatchPrepRename()
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
        IActionResult result = await PrepController.Edit(prepId, jsonPatch);
        Assert.IsType<NoContentResult>(result, exactMatch: false);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId && p.PrepName == "New Prep Name");
    }

    [Fact]
    public async Task TestPatchPrepRenameBadPrepId()
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
        Assert.Equal((int)HttpStatusCode.NotFound, error.StatusCode);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId && p.PrepName != "New Prep Name");
    }

    [Fact]
    public async Task TestPatchPrepEditIdExistingPrepIdShouldError()
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
        Assert.Equal((int)HttpStatusCode.BadRequest, error.StatusCode);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId);
        Assert.Single(preps, p => p.PrepId == badPrepId);
    }

    [Fact]
    public async Task TestPatchPrepEditIdShouldError()
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
        Error result = await PrepController.Edit(prepId, jsonPatch).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId);
        Assert.DoesNotContain(preps, p => p.PrepId == Ulid.NotFound);
    }

    [Fact]
    public async Task TestPatchPrepRemoveNameShouldError()
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
        Error result = await PrepController.Edit(prepId, jsonPatch).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, p => p.PrepId == prepId && p.PrepName != "New Prep Name");
    }

    [Fact]
    public async Task TestDeletePrepInUse()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToList();

        IActionResult result = await PrepController.Delete(SeedData.Preps[3].PrepId);
        Assert.IsType<NoContentResult>(result, exactMatch: false);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count - 1, preps.Count);
        Assert.DoesNotContain(preps, p => Equals(p, Prep.ToResponse.Compile()(SeedData.Preps[3])));
    }
    
    [Fact]
    public async Task TestDeletePrepNotInUse()
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
        IActionResult addResult = await PrepController.Add(addRequest);
        Assert.IsType<NoContentResult>(addResult, exactMatch: false);
        
        List<PrepResponse> prepsAfterAdd = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count + 1, prepsAfterAdd.Count);
        Assert.Contains(prepsAfterAdd, p => p.PrepName == addRequest.PrepName);
        
        Ulid prepId = prepsAfterAdd.Single(p => p.PrepName == addRequest.PrepName).PrepId;

        IActionResult result = await PrepController.Delete(prepId);
        Assert.IsType<NoContentResult>(result, exactMatch: false);
        
        Error errorResult = await PrepController.Delete(prepId).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, errorResult.StatusCode);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync<List<PrepResponse>>();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.DoesNotContain(preps, p => p.PrepName == addRequest.PrepName);
    }
    
    [Fact]
    public async Task TestDeletePrepNotFound()
    {
        Error result = await PrepController.Delete(Ulid.NotFound).ErrorAsync();
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
    }
}