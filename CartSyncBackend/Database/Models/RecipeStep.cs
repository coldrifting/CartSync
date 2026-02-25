using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeStepId))]
public class RecipeStep
{
    public Ulid RecipeStepId { get; set; }

    public Ulid RecipeId { get; set; }
    public string RecipeStepContent { get; set; } = String.Empty;
    public int RecipeStepOrder { get; set; }
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