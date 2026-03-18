using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSyncBackend.Database.Interfaces;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeInstructionId))]
public class RecipeInstruction : ISortable, IEditable<RecipeInstructionEditRequest>
{
    public Ulid RecipeInstructionId { get; set; } = Ulid.NewUlid();

    public Ulid RecipeId { get; set; }
    
    [StringLength(2048)]
    public string RecipeInstructionContent { get; set; } = string.Empty;
    public bool IsImage { get; set; }
    public int SortOrder { get; set; }
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(RecipeId))] 
    public Recipe Recipe
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Recipe));
    }
    
    // Projections
    public RecipeInstructionResponse ToNewResponse =>
        new()
        {
            RecipeInstructionId = RecipeInstructionId,
            RecipeInstructionContent = RecipeInstructionContent,
            IsImage = IsImage,
            SortOrder = SortOrder
        };
    
    public static Expression<Func<RecipeInstruction, RecipeInstructionResponse>> ToResponse =>
        recipeInstruction => new RecipeInstructionResponse
        {
            RecipeInstructionId = recipeInstruction.RecipeInstructionId,
            RecipeInstructionContent = recipeInstruction.RecipeInstructionContent,
            SortOrder = recipeInstruction.SortOrder,
            IsImage = recipeInstruction.IsImage
        };
    
    // Conversion and Validation
    public RecipeInstructionEditRequest ToEditRequest()
    {
        return new RecipeInstructionEditRequest()
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
        SortOrder = editRequest.SortOrder;
        
        int oldIndex = SortOrder;
        Recipe.RecipeSections.Reorder(oldIndex, editRequest.SortOrder);
        //SortOrder = editRequest.SortOrder;
    }
    
    // Errors
    public static NotFoundObjectResult NotFound(Ulid recipeInstructionId) => 
        Error.NotFound(recipeInstructionId, "Recipe Instruction");
    
    public static NotFoundObjectResult NotFoundUnderRecipe(Ulid recipeInstructionId, Ulid recipeId) => 
        Error.NotFoundUnder(recipeInstructionId, "Recipe Instruction", recipeId, "Recipe");
}

public class RecipeInstructionResponse
{
    public Ulid RecipeInstructionId { get; set; }
    public string RecipeInstructionContent { get; set; } = string.Empty;
    public bool IsImage { get; set; }
    public int SortOrder { get; set; }
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