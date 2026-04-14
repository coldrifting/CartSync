using System.ComponentModel.DataAnnotations;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Interfaces;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(PrepId))]
public class Prep : IPatchable<PrepEditRequest>
{
    public Ulid PrepId { get; init; } = Ulid.NewUlid();
    
    [StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; set; }

    // Navigation
    public List<Item> Items { get; init; } = [];
    public List<ItemPrep> ItemPreps { get; init; } = [];
    public List<RecipeEntry> RecipeSectionEntries { get; init; } = [];
    
    // Patch
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<PrepEditRequest> jsonPatch, out PrepEditRequest patch)
    {
        patch = new PrepEditRequest
        {
            Name = PrepName
        };
        
        return Patch.TryPatch(modelState, this, jsonPatch, ref patch);
    }

    public void ApplyPatch(PrepEditRequest patch)
    {
        PrepName = patch.Name;
    }
    
    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid prepId) => 
        ErrorResponse.NotFound(prepId, "Prep");
    
    public static NotFound<ErrorResponse> NotFoundUnder(Ulid prepId, Ulid itemId) => 
        ErrorResponse.NotFoundUnder(prepId, "Prep", itemId, "Item");
    
    public static InvalidOperationException NotLoaded => new("Prep Navigation Property was not loaded");
}