using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Locations")]
public class StoreController(CartSyncContext db) : ControllerCore
{
    [HttpGet]
    [Route("/api/stores")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StoreResponse>))]
    public async Task<IActionResult> All()
    {
        List<StoreResponse> stores = await db.Stores
            .OrderBy(s => s.StoreName)
            .Select(Store.ToStoreResponse)
            .ToListAsync();

        return Ok(stores);
    }
    
    [HttpPost]
    [Route("/api/stores/add")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<IActionResult> Add([FromBody] StoreAddRequest storeAddRequest)
    {
        await db.AddAsync(new Store
        {
            StoreName = storeAddRequest.StoreName,
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPatch]
    [Route("/api/stores/{storeId}/edit")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid storeId, [FromBody] JsonPatchDocument<StoreEditRequest> storePatch)
    {
        Store? store = await db.Stores.FindAsync(storeId);
        if (store == null)
        {
            return Store.NotFound(storeId);
        }

        if (!TryGetEditObject(store, storePatch, out StoreEditRequest? storeEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }

        store.UpdateFromEditRequest(storeEdit);
        await db.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete]
    [Route("/api/stores/{storeId}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid storeId)
    {
        Store? store = await db.Stores.FindAsync(storeId);
        if (store == null)
        {
            return Store.NotFound(storeId);
        }

        db.Stores.Remove(store);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}