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

[PrimaryKey(nameof(RecipeSectionId))]
public class RecipeSection : ISortable, IEditable<RecipeSectionEditRequest>
{
    public Ulid RecipeSectionId { get; set; } = Ulid.NewUlid();
    
    public Ulid RecipeId { get; set; }
    public int SortOrder { get; set; }

    [StringLength(255, MinimumLength = 1)] 
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
    
    // Projections
    public static Expression<Func<RecipeSection, RecipeSectionResponse>> ToResponse =>
        recipeSection => new RecipeSectionResponse
        {
            RecipeSectionId = recipeSection.RecipeSectionId,
            RecipeSectionName = recipeSection.RecipeSectionName,
            SortOrder = recipeSection.SortOrder,
            RecipeSectionEntries = recipeSection.RecipeSectionEntries
                .AsQueryable()
                .OrderBy(rse => rse.SortOrder)
                .Select(RecipeSectionEntry.ToResponse)
                .ToList()
        };
    
    // Conversion and Validation
    public RecipeSectionEditRequest ToEditRequest()
    {
        return new RecipeSectionEditRequest
        {
            RecipeSectionName = RecipeSectionName,
            SortOrder = SortOrder
        };
    }
    
    /// Requires RecipeSection.Recipe.RecipeSections navigation to work
    public void UpdateFromEditRequest(RecipeSectionEditRequest editRequest)
    {
        int oldIndex = SortOrder;
        Recipe.RecipeSections.Reorder(oldIndex, editRequest.SortOrder);
        //SortOrder = editRequest.SortOrder;
    }
    
    // Errors
    public static NotFoundObjectResult NotFound(Ulid recipeSectionId) => 
        Error.NotFound(recipeSectionId, "Recipe Section");
    
    public static NotFoundObjectResult NotFoundUnderRecipe(Ulid recipeSectionId, Ulid recipeId) => 
        Error.NotFoundUnder(recipeSectionId, "Recipe Section", recipeId, "Recipe");
}

public class RecipeSectionResponse
{
    public Ulid? RecipeSectionId { get; set; }
    public string RecipeSectionName { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<RecipeSectionEntryResponse> RecipeSectionEntries { get; set; } = [];
}

public class RecipeSectionEditRequest
{
    [StringLength(255, MinimumLength = 1)] 
    public required string RecipeSectionName { get; set; }
    public required int SortOrder { get; set; }
}