using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database;
using CartSyncBackend.Database.Models;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Controllers;

[ApiController]
[Tags("Preps")]
[Route("/api/preps/[action]")]
public class PrepController(CartSyncContext db) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PrepResponse>))]
    public async Task<IActionResult> All()
    {
        List<PrepResponse> preps = await db.Preps
            .Select(p => new PrepResponse
            {
                PrepId = p.PrepId,
                PrepName = p.PrepName
            })
            .ToListAsync();
        
        return Ok(preps);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    public async Task<IActionResult> Add([Required] string prepName)
    {
        if (!ModelState.IsValid || prepName.Length == 0)
        {
            return Error.BadRequestPrepNameInvalid;
        }
        
        db.Preps.Add(new Prep { PrepName = prepName });
        await db.SaveChangesAsync();
        
        return NoContent();
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Edit([Required] Ulid prepId, [Required] string prepName)
    {
        Prep? prep = await db.Preps.FindAsync(prepId);
        if (prep == null)
        {
            return Error.NotFoundPrep;
        }
        
        if (!ModelState.IsValid || prepName.Length == 0)
        {
            return Error.BadRequestPrepNameInvalid;
        }
        
        prep.PrepName = prepName;
        await db.SaveChangesAsync();
        
        return NoContent();
    }
    
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Error))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Error))]
    public async Task<IActionResult> Delete([Required] Ulid prepId, bool enableOverride = false)
    {
        Prep? prep = await db.Preps.FindAsync(prepId);
        if (prep == null)
        {
            return Error.NotFoundPrep;
        }

        HashSet<ItemResponseNoPrep> itemsUsingPrep = db.Preps
            .Where(p => p.PrepId == prepId)
            .SelectMany(p => p.Items)
            .Select(i => new ItemResponseNoPrep
            {
                ItemId =  i.ItemId,
                ItemName = i.ItemName,
                ItemTemp =  i.ItemTemp,
            })
            .ToHashSet();
        
        if (!enableOverride && itemsUsingPrep.Count != 0)
        {
            Dictionary<string, object> errorDetails = new()
            {
                ["items"] = itemsUsingPrep
            };

            return Error.BadRequestPrepDeleteFailed(errorDetails);
        }
        
        db.Remove(prep);
        await db.SaveChangesAsync();
        
        return NoContent();
    }
}