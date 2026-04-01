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

[PrimaryKey(nameof(RecipeInstructionId))]
public class RecipeInstruction : ISortable, IEditable<RecipeInstructionEditRequest>, IResponse<RecipeInstruction, RecipeInstructionResponse>
{
    public Ulid RecipeInstructionId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeId { get; init; }
    
    [StringLength(2048)]
    public string RecipeInstructionContent { get; set; } = string.Empty;
    public bool IsImage { get; set; }
    public int SortOrder { get; set; }
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(RecipeId))] 
    public Recipe Recipe { set; get => field ?? throw Recipe.NotLoaded; }
    
    // Projections
    public static Expression<Func<RecipeInstruction, RecipeInstructionResponse>> ToResponse =>
        recipeInstruction => new RecipeInstructionResponse
        {
            RecipeInstructionId = recipeInstruction.RecipeInstructionId,
            RecipeInstructionContent = recipeInstruction.RecipeInstructionContent,
            SortOrder = recipeInstruction.SortOrder,
            IsImage = recipeInstruction.IsImage
        };
    
    public RecipeInstructionResponse ToNewResponse =>
        new()
        {
            RecipeInstructionId = RecipeInstructionId,
            RecipeInstructionContent = RecipeInstructionContent,
            IsImage = IsImage,
            SortOrder = SortOrder
        };
    
    // Conversion and Validation
    public RecipeInstructionEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new RecipeInstructionEditRequest
        {
            RecipeInstructionContent = RecipeInstructionContent,
            IsImage = IsImage,
            SortOrder = SortOrder
        };
    }
    
    /// Requires RecipeInstruction.Recipe.RecipeInstructions navigation to work
    public void UpdateFromEditRequest(RecipeInstructionEditRequest editRequest)
    {
        RecipeInstructionContent = editRequest.RecipeInstructionContent;
        IsImage = editRequest.IsImage;
        
        int oldIndex = SortOrder;
        Recipe.RecipeInstructions.Reorder(oldIndex, editRequest.SortOrder);
    }
    
    // Errors
    public static NotFound<Error> NotFound(Ulid recipeInstructionId) => 
        Error.NotFound(recipeInstructionId, "Recipe Instruction");
    
    public static NotFound<Error> NotFoundUnderRecipe(Ulid recipeInstructionId, Ulid recipeId) => 
        Error.NotFoundUnder(recipeInstructionId, "Recipe Instruction", recipeId, "Recipe");
}

public record RecipeInstructionResponse
{
    public required Ulid RecipeInstructionId { get; init; }
    public required string RecipeInstructionContent { get; init; }
    public required bool IsImage { get; init; }
    public required int SortOrder { get; init; }
}

public class RecipeInstructionAddRequest
{
    public required string RecipeInstructionContent { get; set; } = string.Empty;
    public required bool IsImage { get; set; }
}

public class RecipeInstructionEditRequest
{
    public required string RecipeInstructionContent { get; set; }
    public required bool IsImage { get; set; }
    public required int SortOrder { get; set; }
}