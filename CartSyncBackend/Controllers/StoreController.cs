using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Stores")]
[Route("/api/stores/[action]")]
public class StoreController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StoreResponse>))]
    public async Task<IActionResult> All()
    {
        List<StoreResponse> stores = await db.Stores
            .OrderBy(s => s.StoreName)
            .Select(s => new StoreResponse
            {
                StoreId = s.StoreId,
                StoreName = s.StoreName
            })
            .ToListAsync();

        return Ok(stores);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<IActionResult> Add([Required] string storeName)
    {
        if (!ModelState.IsValid || storeName.Length == 0)
        {
            return Error.BadRequestStoreNameInvalid;
        }
        
        await db.AddAsync(new Store
        {
            StoreName = storeName
        });
        await db.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit([Required] Ulid storeId, [Required] string storeName)
    {
        if (!ModelState.IsValid && storeId == Ulid.Empty)
        {
            return Error.BadRequestInvalidStoreId;
        }
        
        Store? s = await db.Stores.FirstOrDefaultAsync(s => s.StoreId == storeId);
        if (s == null)
        {
            return Error.NotFoundStore;
        }
        
        if (!ModelState.IsValid || storeName.Length == 0)
        {
            return Error.BadRequestStoreNameInvalid;
        }
        
        s.StoreName = storeName;
        await db.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete([Required] Ulid storeId)
    {
        if (!ModelState.IsValid && storeId == Ulid.Empty)
        {
            return Error.BadRequestInvalidStoreId;
        }
        
        Store? s = await db.Stores.FirstOrDefaultAsync(s => s.StoreId == storeId);
        if (s == null)
        {
            return Error.NotFoundStore;
        }

        db.Stores.Remove(s);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}