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

[PrimaryKey(nameof(RecipeStepId))]
public class RecipeStep : ISortable, IPatchable<RecipeStepEditRequest>
{
    public Ulid RecipeStepId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeId { get; init; }
    
    [StringLength(2048)]
    public string RecipeStepContent { get; set; } = string.Empty;
    public bool IsImage { get; set; }
    public int SortOrder { get; set; }
    
    // Navigation
    [ForeignKey(nameof(RecipeId))] 
    public Recipe Recipe { init; get => field ?? throw Recipe.NotLoaded; }
    
    // Patch
    public bool TryGetPatch(ModelStateDictionary modelState, JsonPatchDocument<RecipeStepEditRequest> jsonPatch, out RecipeStepEditRequest patch)
    {
        patch = new RecipeStepEditRequest
        {
            Content = RecipeStepContent,
            IsImage = IsImage,
            SortOrder = SortOrder
        };
        
        return Patch.TryPatch(modelState, this, jsonPatch, ref patch);
    }

    public void ApplyPatch(RecipeStepEditRequest patch)
    {
        RecipeStepContent = patch.Content;
        IsImage = patch.IsImage;
        
        Sort.Reorder(Recipe.Steps, SortOrder, patch.SortOrder);
    }
    
    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid recipeStepId) => 
        ErrorResponse.NotFound(recipeStepId, "Recipe Step");
}