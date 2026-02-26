using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeId))]
public class Recipe
{
    public Ulid RecipeId { get; set; } = Ulid.NewUlid();
    
    [StringLength(255)]
    public string RecipeName { get; set; } = string.Empty;
    
    [StringLength(255)]
    public string Url { get; set; } = string.Empty;
    
    public bool IsPinned { get; set; }
    public int CartAmount { get; set; }

    // Navigation
    [JsonIgnore] 
    public List<RecipeInstruction> RecipeInstructions { get; set; } = [];

    [JsonIgnore] 
    public List<RecipeSection> RecipeSections  { get; set; } = [];
}

public class RecipeResponse
{
    public Ulid RecipeId { get; set; }
    
    public string RecipeName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public int CartAmount { get; set; }

    public List<RecipeInstructionResponse> RecipeInstructionsResponse { get; set; } = [];
    public List<RecipeSectionResponse> RecipeSectionsResponse { get; set; } = [];
}

public class RecipeEditRequest
{
    public string? RecipeName  { get; set; }
    public string? Url { get; set; }
    public bool? IsPinned { get; set; }
    public int? CartAmount { get; set; }
}