using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CartSyncBackend.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeSectionId))]
public class RecipeSection : ISortable
{
    public Ulid RecipeSectionId { get; set; } = Ulid.NewUlid();
    
    public Ulid RecipeId { get; set; }
    
    public int SortOrder { get; set; }

    [StringLength(255)] 
    public string RecipeSectionName { get; set; } = string.Empty;
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(RecipeId))]
    public Recipe Recipe
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Recipe));
    }

    public List<RecipeSectionEntry> RecipeSectionEntries { get; set; } = [];
}

public class RecipeSectionResponse
{
    public Ulid? RecipeSectionId { get; set; }
    public int SortOrder { get; set; }
    public string RecipeSectionName { get; set; } = string.Empty;
    public List<RecipeSectionEntryResponse> RecipeSectionEntries { get; set; } = [];
}

public class RecipeSectionEditRequest
{
    public int? SortOrder { get; set; }
    public string? RecipeSectionName { get; set; }
}