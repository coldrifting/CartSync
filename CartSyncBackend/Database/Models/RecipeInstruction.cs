using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeInstructionId))]
public class RecipeInstruction
{
    public Ulid RecipeInstructionId { get; set; } = Ulid.NewUlid();

    public Ulid RecipeId { get; set; }
    
    [StringLength(2048)]
    public string RecipeInstructionContent { get; set; } = string.Empty;
    public int RecipeInstructionIndex { get; set; }
    public bool IsImage { get; set; }
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(RecipeId))] 
    public Recipe Recipe
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Recipe));
    }
}

public class RecipeInstructionResponse
{
    public Ulid RecipeInstructionId { get; set; }
    public string RecipeInstructionContent { get; set; } = string.Empty;
    public int RecipeInstructionIndex { get; set; }
    public bool IsImage { get; set; }
}

public class RecipeInstructionAddRequest
{
    public string RecipeInstructionContent { get; set; } = string.Empty;
    public bool IsImage { get; set; }
}


public class RecipeInstructionEditRequest
{
    public string? RecipeInstructionContent { get; set; }
    public bool? IsImage { get; set; }
    public int? RecipeInstructionIndex { get; set; }
}