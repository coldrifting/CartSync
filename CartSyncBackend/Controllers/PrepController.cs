using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Items")]
public class PrepController(CartSyncContext db) : ControllerCore
{
    [HttpGet]
    [Route("/api/preps")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PrepResponse>))]
    public async Task<IActionResult> All()
    {
        List<PrepResponse> preps = await db.Preps
            .Select(Prep.ToResponse)
            .ToListAsync();
        
        return Ok(preps);
    }

    [HttpPost]
    [Route("/api/preps/add")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<IActionResult> Add([FromBody] PrepAddRequest prepAddRequest)
    {
        db.Preps.Add(new Prep { PrepName = prepAddRequest.PrepName });
        await db.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPatch]
    [Route("/api/preps/{prepId}/edit")]
    [Consumes("application/json-patch+json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit(Ulid prepId, [FromBody] JsonPatchDocument<PrepEditRequest> prepPatch)
    {
        Prep? prep = await db.Preps.FindAsync(prepId);
        if (prep == null)
        {
            return Prep.NotFound(prepId);
        }
        
        if (!TryGetEditObject(prep, prepPatch, out PrepEditRequest? prepEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }
        
        prep.UpdateFromEditRequest(prepEdit);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete]
    [Route("/api/preps/{prepId}/delete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete(Ulid prepId)
    {
        Prep? prep = await db.Preps.FindAsync(prepId);
        if (prep == null)
        {
            return Prep.NotFound(prepId);
        }
        
        db.Remove(prep);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}