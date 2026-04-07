using System.Net;
using CartSync.Controllers.Core;
using CartSync.Models;
using CartSyncTests.Base;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using SeedData = CartSync.Models.Seeding.SeedData;

namespace CartSyncTests.ControllerTests;

[Collection("DatabaseTests")]
public class AisleControllerTests(DatabaseSetup fixture) : DatabaseFixture(fixture)
{
    [Fact]
    public async Task TestAisleAll()
    {
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync();
        Assert.Equal(23, aisles.Count);
        Assert.Contains((SeedData.Aisles[0].AisleId, SeedData.Aisles[0].AisleName), aisles.Select(aisle => (aisle.Id, aisle.Name)));
        Assert.Contains((SeedData.Aisles[5].AisleId, SeedData.Aisles[5].AisleName), aisles.Select(aisle => (aisle.Id, aisle.Name)));
        Assert.Contains((SeedData.Aisles[13].AisleId, SeedData.Aisles[13].AisleName), aisles.Select(aisle => (aisle.Id, aisle.Name)));
        Assert.Contains((SeedData.Aisles[22].AisleId, SeedData.Aisles[22].AisleName), aisles.Select(aisle => (aisle.Id, aisle.Name)));
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync();
        Assert.Single(aisles2);
        Assert.Contains((SeedData.Aisles[23].AisleId, SeedData.Aisles[23].AisleName), aisles2.Select(aisle => (aisle.Id, aisle.Name)));
    }

    [Fact]
    public async Task TestAisleAll_StoreIdNotFound()
    {
        Error error = await AisleController.All(Ulid.NotFound).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestAisleAdd()
    {
        AisleAddRequest aisleNewRequest = new()
        {
            Name = "New Aisle"
        };
        (AisleResponse aisle, string location) result = await AisleController.Add(SeedData.Stores[0].StoreId, aisleNewRequest).ValueAsync();
        Assert.Equal(aisleNewRequest.Name, result.aisle.Name);
        
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync();
        Assert.Equal(24, aisles.Count);
        Assert.Contains(aisleNewRequest.Name, aisles.Select(aisle => aisle.Name));
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync();
        Assert.Single(aisles2);
        Assert.DoesNotContain(aisleNewRequest.Name, aisles2.Select(aisle => aisle.Name));
    }

    [Fact]
    public async Task TestAisleAdd_StoreNotFound()
    {
        Error error = await AisleController.Add(Ulid.NotFound, new AisleAddRequest
        {
            Name = "New Aisle"
        }).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task TestAisleEdit_Rename()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/Name",
                    value = "New Aisle"
                }
            }
        };

        await AisleController.Edit(SeedData.Aisles[4].AisleId, jsonPatch).AssertIsSuccessful();
        
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync();
        Assert.Equal(23, aisles.Count);
        Assert.Contains("New Aisle", aisles.Select(aisle => aisle.Name));
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync();
        Assert.Single(aisles2);
        Assert.DoesNotContain("New Aisle", aisles2.Select(aisle => aisle.Name));
    }

    [Fact]
    public async Task TestAisleEdit_ReorderToFirst()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/SortOrder",
                    value = 0
                }
            }
        };
        
        await AisleController.Edit(SeedData.Aisles[4].AisleId, jsonPatch).AssertIsSuccessful();
        
        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync();
        Assert.Equal(23, aisles.Count);
        Assert.Equal(SeedData.Aisles[4].AisleName, aisles.OrderBy(aisle => aisle.SortOrder).Select(aisle => aisle.Name).FirstOrDefault());
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync();
        Assert.Single(aisles2);
        Assert.DoesNotContain(SeedData.Aisles[4].AisleName, aisles2.Select(aisle => aisle.Name));
    }

    [Fact]
    public async Task TestAisleEdit_AisleNotFound()
    {
        JsonPatchDocument<AisleEditRequest> jsonPatch = new()
        {
            Operations =
            {
                new Operation<AisleEditRequest>
                {
                    op = "replace",
                    path = "/AisleName",
                    value = "New Aisle"
                }
            }
        };
        
        Error error = await AisleController.Edit(Ulid.NotFound, jsonPatch).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        await TestAisleAll();
    }

    [Fact]
    public async Task TestAisleDelete()
    {
        await AisleController.Delete(SeedData.Aisles[2].AisleId).AssertIsSuccessful();

        List<AisleResponse> aisles = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync();
        Assert.Equal(22, aisles.Count);
        Assert.DoesNotContain(SeedData.Aisles[2].AisleId, aisles.Select(aisle => aisle.Id));
        
        List<AisleResponse> aisles2 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync();
        Assert.Single(aisles2);
        Assert.DoesNotContain(SeedData.Aisles[2].AisleId, aisles2.Select(aisle => aisle.Id));
        
        
        await AisleController.Delete(SeedData.Aisles[23].AisleId).AssertIsSuccessful();
        
        List<AisleResponse> aisles3 = await AisleController.All(SeedData.Stores[0].StoreId).ValueAsync();
        Assert.Equal(22, aisles3.Count);
        Assert.DoesNotContain(SeedData.Aisles[23].AisleId, aisles3.Select(aisle => aisle.Id));
        
        List<AisleResponse> aisles4 = await AisleController.All(SeedData.Stores[1].StoreId).ValueAsync();
        Assert.Empty(aisles4);
    }

    [Fact]
    public async Task TestAisleDelete_AisleNotFound()
    {
        Error error = await AisleController.Delete(Ulid.NotFound).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);
        
        await TestAisleAll();
    }

    [Fact]
    public async Task TestAisleDelete_InvalidAisleId()
    {
        Error error = await AisleController.Delete(SeedData.Stores[0].StoreId).ErrorAsync();
        error.AssertStatus(HttpStatusCode.NotFound);

        await TestAisleAll();
    }
}