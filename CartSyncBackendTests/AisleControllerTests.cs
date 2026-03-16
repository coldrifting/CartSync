using System.Net;
using CartSyncBackend.Controllers;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using CartSyncBackendTests.Core;
using Microsoft.AspNetCore.Mvc;
using static CartSyncBackendTests.Core.Constants;
using static CartSyncBackend.Database.Seeding.SeedData;

namespace CartSyncBackendTests;

[Collection("DatabaseUnitTests")]
public class AisleControllerTests(WebAppFactory<Program> factory) : WebAppFixture(factory)
{
    private AisleController _aisleController = null!;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _aisleController = new AisleController(Context);
    }

    [Fact]
    public async Task TestGetAisles()
    {
        List<AisleResponse> aisles = (await _aisleController.All(Stores[0].StoreId)).Value<List<AisleResponse>>();
        Assert.Equal(23, aisles.Count);
        Assert.Contains((Aisles[0].AisleId, Aisles[0].AisleName), aisles.Select(a => (a.AisleId, a.AisleName)));
        Assert.Contains((Aisles[5].AisleId, Aisles[5].AisleName), aisles.Select(a => (a.AisleId, a.AisleName)));
        Assert.Contains((Aisles[13].AisleId, Aisles[13].AisleName), aisles.Select(a => (a.AisleId, a.AisleName)));
        Assert.Contains((Aisles[22].AisleId, Aisles[22].AisleName), aisles.Select(a => (a.AisleId, a.AisleName)));
        
        List<AisleResponse> aisles2 = (await _aisleController.All(Stores[1].StoreId)).Value<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.Contains((Aisles[23].AisleId, Aisles[23].AisleName), aisles2.Select(a => (a.AisleId, a.AisleName)));
    }

    [Fact]
    public async Task TestGetAislesStoreIdNotFound()
    {
        Error error = (await _aisleController.All(NotFoundId)).Error();
        Assert.Equal(BadRequestStatusCode, error.Status);
        
        await TestGetAisles();
    }
    
    [Fact]
    public async Task TestGetAislesStoreIdInvalid()
    {
        HttpResponseMessage response = await Client.GetAsync($"api/aisles/all?storeId={BadIdString}");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task TestAddAisle()
    {
        IActionResult result = await _aisleController.Add(new AisleAddRequest
        {
            AisleName = "New Aisle",
            StoreId = Stores[0].StoreId
        });
        Assert.IsType<NoContentResult>(result);
        
        List<AisleResponse> aisles = (await _aisleController.All(Stores[0].StoreId)).Value<List<AisleResponse>>();
        Assert.Equal(24, aisles.Count);
        Assert.Contains("New Aisle", aisles.Select(a => a.AisleName));
        
        List<AisleResponse> aisles2 = (await _aisleController.All(Stores[1].StoreId)).Value<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.DoesNotContain("New Aisle", aisles2.Select(a => a.AisleName));
    }

    [Fact]
    public async Task TestAddAisleInvalidStoreId()
    {
        Error error = (await _aisleController.Add(new AisleAddRequest
        {
            AisleName = "New Aisle",
            StoreId = NotFoundId
        })).Error();
        Assert.Equal(BadRequestStatusCode, error.Status);
        
        await TestGetAisles();
    }

    [Fact]
    public async Task TestEditAisleRename()
    {
        IActionResult result = await _aisleController.Edit(Aisles[4].AisleId, new AisleEditRequest
        {
            AisleName = "New Aisle"
        });
        Assert.IsType<NoContentResult>(result);
        
        List<AisleResponse> aisles = (await _aisleController.All(Stores[0].StoreId)).Value<List<AisleResponse>>();
        Assert.Equal(23, aisles.Count);
        Assert.Contains("New Aisle", aisles.Select(a => a.AisleName));
        
        List<AisleResponse> aisles2 = (await _aisleController.All(Stores[1].StoreId)).Value<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.DoesNotContain("New Aisle", aisles2.Select(a => a.AisleName));
    }

    [Fact]
    public async Task TestEditAisleReorderFirst()
    {
        IActionResult result = await _aisleController.Edit(Aisles[4].AisleId, new AisleEditRequest
        {
            SortOrder = 0
        });
        Assert.IsType<NoContentResult>(result);
        
        List<AisleResponse> aisles = (await _aisleController.All(Stores[0].StoreId)).Value<List<AisleResponse>>();
        Assert.Equal(23, aisles.Count);
        Assert.Equal(Aisles[4].AisleName, aisles.OrderBy(a => a.SortOrder).Select(a => a.AisleName).FirstOrDefault());
        
        List<AisleResponse> aisles2 = (await _aisleController.All(Stores[1].StoreId)).Value<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.DoesNotContain(Aisles[4].AisleName, aisles2.Select(a => a.AisleName));
    }

    [Fact]
    public async Task TestEditAisleInvalidAisleId()
    {
        Error error = (await _aisleController.Edit(NotFoundId, new AisleEditRequest
        {
            AisleName = "New Aisle",
            SortOrder = -1
        })).Error();
        Assert.Equal(NotFoundStatusCode, error.Status);
        
        await TestGetAisles();
    }

    [Fact]
    public async Task TestDeleteAisle()
    {
        IActionResult result = await _aisleController.Delete(Aisles[2].AisleId);
        Assert.IsType<NoContentResult>(result);
        
        List<AisleResponse> aisles = (await _aisleController.All(Stores[0].StoreId)).Value<List<AisleResponse>>();
        Assert.Equal(22, aisles.Count);
        Assert.DoesNotContain(Aisles[2].AisleId, aisles.Select(a => a.AisleId));
        
        List<AisleResponse> aisles2 = (await _aisleController.All(Stores[1].StoreId)).Value<List<AisleResponse>>();
        Assert.Single(aisles2);
        Assert.DoesNotContain(Aisles[2].AisleId, aisles2.Select(a => a.AisleId));
        
        
        IActionResult result2 = await _aisleController.Delete(Aisles[23].AisleId);
        Assert.IsType<NoContentResult>(result2);
        
        List<AisleResponse> aisles3 = (await _aisleController.All(Stores[0].StoreId)).Value<List<AisleResponse>>();
        Assert.Equal(22, aisles3.Count);
        Assert.DoesNotContain(Aisles[23].AisleId, aisles3.Select(a => a.AisleId));
        
        List<AisleResponse> aisles4 = (await _aisleController.All(Stores[1].StoreId)).Value<List<AisleResponse>>();
        Assert.Empty(aisles4);
    }

    [Fact]
    public async Task TestDeleteAisleInvalidAisleId()
    {
        Error error = (await _aisleController.Delete(NotFoundId)).Error();
        Assert.Equal(NotFoundStatusCode, error.Status);
        
        await TestGetAisles();
    }

    [Fact]
    public async Task TestDeleteAisleStoreIdInsteadOfAisleId()
    {
        Error error = (await _aisleController.Delete(Stores[0].StoreId)).Error();
        Assert.Equal(NotFoundStatusCode, error.Status);

        await TestGetAisles();
    }
}