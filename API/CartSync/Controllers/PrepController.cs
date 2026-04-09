using CartSync.Controllers.Core;
using CartSync.Data.Entities;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Database;
using CartSync.Objects;
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
    public async Task<Ok<ReadOnlyList<PrepResponse>>> All()
    {
        ReadOnlyList<PrepResponse> preps = await Db.Preps
            .Select(PrepResponse.FromEntity)
            .OrderBy(prep => prep.Name)
            .ThenBy(prep => prep.Id)
            .ToReadOnlyListAsync();
        
        return TypedResults.Ok(preps);
    }

    [HttpPost]
    [Route("/api/preps/add")]
    public async Task<Results<Created<PrepResponse>, BadRequest<ErrorResponse>>> Add(AddRequest addRequest)
    {
        Prep prep = new()
        {
            PrepName = addRequest.Name
        };
        
        Db.Add(prep);
        await Db.SaveChangesAsync();
        
        return TypedResults.Created($"/api/preps/{prep.PrepId}", PrepResponse.FromNewEntity(prep));
    }
    
    [HttpGet]
    [Route("/api/preps/{prepId}/usages")]
    public async Task<Results<Ok<PrepUsagesResponse>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Usages(Ulid prepId)
    {
        PrepUsagesResponse? prepUsages = await Db.Preps
            .Include(p => p.Items)
            .AsSplitQuery()
            .Include(p => p.RecipeSectionEntries)
            .ThenInclude(r => r.RecipeSection)
            .ThenInclude(r => r.Recipe)
            .Select(PrepUsagesResponse.FromEntity)
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
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Edit(Ulid prepId, JsonPatchDocument<PrepEditRequest> jsonPatch)
    {
        Prep? prep = await Db.Preps.FindAsync(prepId);
        if (prep == null)
        {
            return Prep.NotFound(prepId);
        }

        if (!prep.TryGetPatch(ModelState, jsonPatch, out PrepEditRequest patch))
        {
            return ErrorResponse.BadRequestPatchInvalid(ModelState);
        }

        prep.ApplyPatch(patch);
        await Db.SaveChangesAsync();
        
        return TypedResults.NoContent();
    }
    
    [HttpDelete]
    [Route("/api/preps/{prepId}/delete")]
    public async Task<Results<NoContent, BadRequest<ErrorResponse>, NotFound<ErrorResponse>>> Delete(Ulid prepId)
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