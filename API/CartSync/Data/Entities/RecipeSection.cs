using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CartSync.Data.Requests;
using CartSync.Data.Responses;
using CartSync.Interfaces;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(RecipeSectionId))]
public class RecipeSection : ISortable, IPatchable<RecipeSectionEditRequest>
{
    public Ulid RecipeSectionId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeId { get; init; }
    public int SortOrder { get; set; }

    [StringLength(255, MinimumLength = 1)] 
    public required string RecipeSectionName { get; set; }
    
    // Navigation
    [ForeignKey(nameof(RecipeId))]
    public Recipe Recipe { init; get => field ?? throw Recipe.NotLoaded; }

    public List<RecipeEntry> Entries { get; init; } = [];
    
    // Patch
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<RecipeSectionEditRequest> jsonPatch, out RecipeSectionEditRequest patch)
    {
        patch = new RecipeSectionEditRequest
        {
            Name = RecipeSectionName,
            SortOrder = SortOrder
        };
        
        return Patch.TryPatch(modelState, this, jsonPatch, ref patch);
    }

    public void ApplyPatch(RecipeSectionEditRequest patch)
    {
        RecipeSectionName = patch.Name;
        
        Sort.Reorder(Recipe.Sections, SortOrder, patch.SortOrder);
    }
    
    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid recipeSectionId) => 
        ErrorResponse.NotFound(recipeSectionId, "Recipe Section");
    
    public static InvalidOperationException NotLoaded => new("Recipe Section Property was not loaded");
}