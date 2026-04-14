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

[PrimaryKey(nameof(RecipeId))]
public class Recipe : IPatchable<RecipeEditRequest>
{
    public Ulid RecipeId { get; init; } = Ulid.NewUlid();
    
    [StringLength(255, MinimumLength = 1)]
    public required string RecipeName { get; set; }
    
    [StringLength(2048)]
    public string Url { get; set; } = string.Empty;
    
    public bool IsPinned { get; set; }
    public int CartQuantity { get; set; }

    // Navigation
    public List<RecipeStep> Steps { get; init; } = [];
    public List<RecipeSection> Sections  { get; init; } = [];
    
    // Patch
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<RecipeEditRequest> jsonPatch, out RecipeEditRequest patch)
    {
        patch = new RecipeEditRequest
        {
            Name = RecipeName,
            Url = Url,
            IsPinned = IsPinned,
        };
        
        return Patch.TryPatch(modelState, this, jsonPatch, ref patch);
    }

    public void ApplyPatch(RecipeEditRequest patch)
    {
        RecipeName = patch.Name;
        Url = patch.Url;
        IsPinned = patch.IsPinned;
    }
    
    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid recipeId) => 
        ErrorResponse.NotFound(recipeId, "Recipe");
    
    public static InvalidOperationException NotLoaded => new("Recipe Navigation Property was not loaded");
}