using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(RecipeStepId))]
public class RecipeStep : ISortable, IEditable<RecipeStepEditRequest>, IResponse<RecipeStep, RecipeStepResponse>
{
    public Ulid RecipeStepId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeId { get; init; }
    
    [StringLength(2048)]
    public string RecipeStepContent { get; set; } = string.Empty;
    public bool IsImage { get; set; }
    public int SortOrder { get; set; }
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(RecipeId))] 
    public Recipe Recipe { init; get => field ?? throw Recipe.NotLoaded; }
    
    // Projections
    public static Expression<Func<RecipeStep, RecipeStepResponse>> ToResponse =>
        recipeStep => new RecipeStepResponse
        {
            Id = recipeStep.RecipeStepId,
            Content = recipeStep.RecipeStepContent,
            SortOrder = recipeStep.SortOrder,
            IsImage = recipeStep.IsImage
        };
    
    public RecipeStepResponse ToNewResponse =>
        new()
        {
            Id = RecipeStepId,
            Content = RecipeStepContent,
            IsImage = IsImage,
            SortOrder = SortOrder
        };
    
    // Conversion and Validation
    public RecipeStepEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new RecipeStepEditRequest
        {
            Content = RecipeStepContent,
            IsImage = IsImage,
            SortOrder = SortOrder
        };
    }
    
    /// Requires RecipeStep.Recipe.RecipeSteps navigation to work
    public void UpdateFromEditRequest(RecipeStepEditRequest editRequest)
    {
        RecipeStepContent = editRequest.Content;
        IsImage = editRequest.IsImage;
        
        int oldIndex = SortOrder;
        Recipe.Steps.Reorder(oldIndex, editRequest.SortOrder);
    }
    
    // Errors
    public static NotFound<Error> NotFound(Ulid recipeStepId) => 
        Error.NotFound(recipeStepId, "Recipe Step");
}

public record RecipeStepResponse
{
    public required Ulid Id { get; init; }
    public required string Content { get; init; }
    public required bool IsImage { get; init; }
    public required int SortOrder { get; init; }
}

public class RecipeStepAddRequest
{
    public required string Content { get; init; } = string.Empty;
    public required bool IsImage { get; init; }
}

public class RecipeStepEditRequest
{
    public required string Content { get; init; }
    public required bool IsImage { get; init; }
    public required int SortOrder { get; init; }
}