using CartSync.Controllers.Core;
using CartSync.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Controllers;

[Tags("Items")]
public class PrepController(CartSyncContext context) : ControllerCore(context)
{
    [HttpGet]
    [Route("/api/preps")]
    public async Task<Ok<List<PrepResponse>>> All()
    {
        List<PrepResponse> preps = await Db.Preps
            .Select(Prep.ToResponse)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .ToListAsync();
        
        return TypedResults.Ok(preps);
    }

    [HttpPost]
    [Route("/api/preps/add")]
    public async Task<Results<Created<PrepResponse>, BadRequest<Error>>> Add([FromBody] PrepAddRequest prepAddRequest)
    {
        Prep prep = new()
        {
            PrepName = prepAddRequest.Name
        };
        
        Db.Add(prep);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/preps/{prep.PrepId}", prep.ToNewResponse);
    }
    
    [HttpGet]
    [Route("/api/preps/{prepId}/usages")]
    public async Task<Results<Ok<PrepUsagesResponse>, BadRequest<Error>, NotFound<Error>>> Usages(Ulid prepId)
    {
        PrepUsagesResponse? prepUsages = await Db.Preps
            .Include(p => p.Items)
            .AsSplitQuery()
            .Include(p => p.RecipeSectionEntries)
            .ThenInclude(r => r.RecipeSection)
            .ThenInclude(r => r.Recipe)
            .Select(Prep.ToUsagesResponse)
            .FirstOrDefaultAsync(prepUsages => prepUsages.Id == prepId);
        if (prepUsages == null)
        {
            return Prep.NotFound(prepId);
        }

        return TypedResults.Ok(prepUsages);
    }

    [HttpPatch]
    [Route("/api/preps/{prepId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid prepId, [FromBody] JsonPatchDocument<PrepEditRequest> prepPatch)
    {
        Prep? prep = await Db.Preps.FindAsync(prepId);
        if (prep == null)
        {
            return Prep.NotFound(prepId);
        }
        
        if (!TryGetEditObject(prep, prepPatch, out PrepEditRequest? prepEdit))
        {
            return Error.BadRequestPatchInvalid(ModelState);
        }
        
        prep.UpdateFromEditRequest(prepEdit);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/preps/{prepId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid prepId)
    {
        Prep? prep = await Db.Preps.FindAsync(prepId);
        if (prep == null)
        {
            return Prep.NotFound(prepId);
        }
        
        Db.Remove(prep);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}