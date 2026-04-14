using System.ComponentModel.DataAnnotations.Schema;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Interfaces;
using CartSync.Objects;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(RecipeEntryId))]
public class RecipeEntry : IPatchable<RecipeEntryEditRequest>
{
    public Ulid RecipeEntryId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeSectionId { get; init; }
    public Ulid ItemId { get; init; }
    public Ulid? PrepId { get; set; }
    public Amount Amount { get; set; } = new();
    
    // Navigation
    [ForeignKey(nameof(RecipeSectionId))]
    public RecipeSection RecipeSection { init; get => field ?? throw RecipeSection.NotLoaded; }
    
    [ForeignKey(nameof(ItemId))]
    public Item Item { init; get => field ?? throw Item.NotLoaded; }

    [ForeignKey(nameof(PrepId))]
    public Prep? Prep { get; init; }
    
    // Patch
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<RecipeEntryEditRequest> jsonPatch, out RecipeEntryEditRequest patch)
    {
        patch = new RecipeEntryEditRequest
        {
            PrepId = PrepId,
            Amount = Amount
        };
        
        return Patch.TryPatch(modelState, this, jsonPatch, ref patch);
    }

    public void ApplyPatch(RecipeEntryEditRequest patch)
    {
        PrepId = patch.PrepId;
        Amount = patch.Amount;
    }
    
    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid recipeSectionEntryId) => 
        ErrorResponse.NotFound(recipeSectionEntryId, "Recipe Section Entry");
    
    public static Conflict<ErrorResponse> AlreadyExists(Ulid itemId, Ulid? prepId) => 
        ErrorResponse.AlreadyExists("Recipe Section Entry", itemId, "Item", prepId, "Prep");
}