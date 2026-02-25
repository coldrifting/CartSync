using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(RecipeId))]
public class Recipe
{
    public Ulid RecipeId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public int CartAmount { get; set; }

    // Navigation
    public List<RecipeStep> RecipeSteps = null!;
}