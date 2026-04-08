using System.Net;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.SeedData.SeedData;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class PrepControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestPrepAll()
    {
        List<PrepResponse> expectedPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
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
            Id = SeedData.Preps[3].PrepId,
            Name = SeedData.Preps[3].PrepName,
            Items = [
                new ItemMinimalResponse
                {
                    Id = SeedData.Items[179].ItemId,
                    Name = SeedData.Items[179].ItemName,
                    Temp = SeedData.Items[179].Temp,
                    DefaultUnitType = SeedData.Items[179].DefaultUnitType
                },
                new ItemMinimalResponse
                {
                    Id = SeedData.Items[180].ItemId,
                    Name = SeedData.Items[180].ItemName,
                    Temp = SeedData.Items[180].Temp,
                    DefaultUnitType = SeedData.Items[180].DefaultUnitType
                },
                new ItemMinimalResponse
                {
                    Id = SeedData.Items[181].ItemId,
                    Name = SeedData.Items[181].ItemName,
                    Temp = SeedData.Items[181].Temp,
                    DefaultUnitType = SeedData.Items[181].DefaultUnitType
                },
                new ItemMinimalResponse
                {
                    Id = SeedData.Items[182].ItemId,
                    Name = SeedData.Items[182].ItemName,
                    Temp = SeedData.Items[182].Temp,
                    DefaultUnitType = SeedData.Items[182].DefaultUnitType
                },
                new ItemMinimalResponse
                {
                    Id = SeedData.Items[183].ItemId,
                    Name = SeedData.Items[183].ItemName,
                    Temp = SeedData.Items[183].Temp,
                    DefaultUnitType = SeedData.Items[183].DefaultUnitType
                },
                new ItemMinimalResponse
                {
                    Id = SeedData.Items[184].ItemId,
                    Name = SeedData.Items[184].ItemName,
                    Temp = SeedData.Items[184].Temp,
                    DefaultUnitType = SeedData.Items[184].DefaultUnitType
                }
            ],
            Recipes = [
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[1].RecipeId,
                    Name = SeedData.Recipes[1].RecipeName,
                    IsPinned = SeedData.Recipes[1].IsPinned,
                    Url = SeedData.Recipes[1].Url
                },
                new RecipeMinimalResponse
                {
                    Id = SeedData.Recipes[3].RecipeId,
                    Name = SeedData.Recipes[3].RecipeName,
                    IsPinned = SeedData.Recipes[3].IsPinned,
                    Url = SeedData.Recipes[3].Url
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
            Id = SeedData.Preps[6].PrepId,
            Name = SeedData.Preps[6].PrepName,
            Items =
            [
                new ItemMinimalResponse
                {
                    Id = SeedData.Items[56].ItemId,
                    Name = SeedData.Items[56].ItemName,
                    Temp = SeedData.Items[56].Temp,
                    DefaultUnitType = SeedData.Items[56].DefaultUnitType
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
        ErrorResponse errorResponse = await PrepController.Usages(Ulid.NotFound).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }
    
    [Fact]
    public async Task TestPrepAdd()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .ToList();

        AddRequest addRequest = new()
        {
            Name = "New Prep Name"
        };
        (PrepResponse prep, string location) result = await PrepController.Add(addRequest).ValueAsync();
        Assert.Equal(addRequest.Name, result.prep.Name);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.prep.Id.ToString().ToLower());
        
        List<PrepResponse> preps = await PrepController.All().ValueAsync();
        
        Assert.Equal(previousPreps.Count + 1, preps.Count);
        Assert.Contains(preps, prep => prep.Name == addRequest.Name);
    }

    [Fact]
    public async Task TestPrepEdit_Rename()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .ToList();
        
        JsonPatchDocument<PrepEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<PrepEditRequest>
                {
                    op = "replace",
                    path = "/Name",
                    value = "New Prep Name"
                }
            }
        };
        
        Ulid prepId = SeedData.Preps[3].PrepId;
        await PrepController.Edit(prepId, jsonPatch).AssertIsSuccessful();
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, prep => prep.Id == prepId && prep.Name == "New Prep Name");
    }

    [Fact]
    public async Task TestPrepEdit_BadPrepId()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
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
        ErrorResponse errorResponse = await PrepController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, prep => prep.Id == prepId && prep.Name != "New Prep Name");
    }

    [Fact]
    public async Task TestPrepEdit_ReplacePrepIdWithBadIdShouldError()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
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
        ErrorResponse errorResponse = await PrepController.Edit(prepId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.BadRequest);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, prep => prep.Id == prepId);
        Assert.Single(preps, prep => prep.Id == badPrepId);
    }

    [Fact]
    public async Task TestPrepEdit_ReplacePrepIdWithNotFoundIdShouldError()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
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
        ErrorResponse errorResponse = await PrepController.Edit(prepId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.BadRequest);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, prep => prep.Id == prepId);
        Assert.DoesNotContain(preps, prep => prep.Id == Ulid.NotFound);
    }

    [Fact]
    public async Task TestPrepEdit_RemoveNameShouldError()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
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
        ErrorResponse errorResponse = await PrepController.Edit(prepId, jsonPatch).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.BadRequest);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.Contains(preps, prep => prep.Id == prepId && prep.Name != "New Prep Name");
    }

    [Fact]
    public async Task TestPrepDelete_Cascade()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .ToList();

        await PrepController.Delete(SeedData.Preps[3].PrepId).AssertIsSuccessful();
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count - 1, preps.Count);
        Assert.DoesNotContain(preps, p => Equals(p, PrepResponse.FromEntity.Compile()(SeedData.Preps[3])));
    }
    
    [Fact]
    public async Task TestPrepDelete_NoForeignKeys()
    {
        List<PrepResponse> previousPreps = SeedData.Preps
            .AsQueryable()
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .ToList();

        AddRequest addRequest = new()
        {
            Name = "New Prep Name"
        };
        (PrepResponse prep, string location) result = await PrepController.Add(addRequest).ValueAsync();
        Assert.Equal(addRequest.Name, result.prep.Name);
        Assert.Equal(result.location.Split('/').Last().ToLower(), result.prep.Id.ToString().ToLower());
        
        List<PrepResponse> prepsAfterAdd = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count + 1, prepsAfterAdd.Count);
        Assert.Contains(prepsAfterAdd, prep => prep.Name == addRequest.Name);
        
        Ulid prepId = prepsAfterAdd.Single(prep => prep.Name == addRequest.Name).Id;

        await PrepController.Delete(prepId).AssertIsSuccessful();
        
        ErrorResponse errorResponse = await PrepController.Delete(prepId).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
        
        List<PrepResponse> preps = await PrepController.All()
            .ValueAsync();
        
        Assert.Equal(previousPreps.Count, preps.Count);
        Assert.DoesNotContain(preps, prep => prep.Name == addRequest.Name);
    }
    
    [Fact]
    public async Task TestPrepDelete_PrepNotFound()
    {
        ErrorResponse errorResponse = await PrepController.Delete(Ulid.NotFound).ErrorAsync();
        errorResponse.AssertStatus(HttpStatusCode.NotFound);
    }
}