using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public async Task<Ok<List<PrepResponse>>> All()
    {
        List<PrepResponse> preps = await db.Preps
            .Select(Prep.ToResponse)
            .OrderBy(p => p.PrepName)
            .ThenBy(p => p.PrepId)
            .ToListAsync();
        
        return TypedResults.Ok(preps);
    }

    [HttpPost]
    [Route("/api/preps/add")]
    public async Task<Results<Created<PrepResponse>, BadRequest<Error>>> Add([FromBody] PrepAddRequest prepAddRequest)
    {
        Prep prep = new()
        {
            PrepName = prepAddRequest.PrepName
        };
        
        await db.Preps.AddAsync(prep);
        await db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/preps/{prep.PrepId}", prep.ToNewResponse);
    }
    
    [HttpGet]
    [Route("/api/preps/{prepId}/usages")]
    public async Task<Results<Ok<UsageResponse>, BadRequest<Error>, NotFound<Error>>> Usages(Ulid prepId)
    {
        Prep? prep = await db.Preps
            .Include(p => p.Items)
            .Include(p => p.ItemPreps)
            .Include(p => p.RecipeSectionEntries)
            .ThenInclude(r => r.RecipeSection)
            .ThenInclude(r => r.Recipe)
            .FirstOrDefaultAsync(p => p.PrepId == prepId);
        if (prep == null)
        {
            return Prep.NotFound(prepId);
        }

        IOrderedEnumerable<Item> items = prep.Items
            .OrderBy(i => i.ItemName)
            .ThenBy(i => i.ItemId);
        
        IEnumerable<Recipe> recipes = prep.RecipeSectionEntries
            .Select(r => r.RecipeSection)
            .Select(r => r.Recipe)
            .Distinct()
            .OrderBy(r => r.RecipeName)
            .ThenBy(r => r.RecipeId);
        
        UsageResponse result = new();
        result.Update(items, i => i.ItemId, i => i.ItemName);
        result.Update(recipes, r => r.RecipeId, r => r.RecipeName);
        
        return TypedResults.Ok(result);
    }

    [HttpPatch]
    [Route("/api/preps/{prepId}/edit")]
    [Consumes("application/json-patch+json")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Edit(Ulid prepId, [FromBody] JsonPatchDocument<PrepEditRequest> prepPatch)
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
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/preps/{prepId}/delete")]
    public async Task<Results<NoContent, BadRequest<Error>, NotFound<Error>>> Delete(Ulid prepId)
    {
        Prep? prep = await db.Preps.FindAsync(prepId);
        if (prep == null)
        {
            return Prep.NotFound(prepId);
        }
        
        db.Remove(prep);
        await db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
}