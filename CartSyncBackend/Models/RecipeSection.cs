using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models.Interfaces;
using CartSyncBackend.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Models;

[PrimaryKey(nameof(RecipeSectionId))]
public class RecipeSection : ISortable, IEditable<RecipeSectionEditRequest>, IResponse<RecipeSection, RecipeSectionResponse>
{
    public Ulid RecipeSectionId { get; init; } = Ulid.NewUlid();
    
    public Ulid RecipeId { get; init; }
    public int SortOrder { get; set; }

    [StringLength(255, MinimumLength = 1)] 
    public required string RecipeSectionName { get; set; }
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(RecipeId))]
    public Recipe Recipe { set; get => field ?? throw Recipe.NotLoaded; }

    public List<RecipeSectionEntry> RecipeSectionEntries { get; init; } = [];
    
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
                .ToImmutableList()
                .WithValueSemantics()
        };
    
    public RecipeSectionResponse ToNewResponse =>
        new()
        {
            RecipeSectionId = RecipeSectionId,
            RecipeSectionName = RecipeSectionName,
            SortOrder = SortOrder,
            RecipeSectionEntries = []
        };

    
    // Conversion and Validation
    public RecipeSectionEditRequest ToEditRequest(Ulid? storeId = null)
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
    }
    
    // Errors
    public static NotFound<Error> NotFound(Ulid recipeSectionId) => 
        Error.NotFound(recipeSectionId, "Recipe Section");
    
    public static NotFound<Error> NotFoundUnderRecipe(Ulid recipeSectionId, Ulid recipeId) => 
        Error.NotFoundUnder(recipeSectionId, "Recipe Section", recipeId, "Recipe");
    
    public static InvalidOperationException NotLoaded => new("Recipe Section Property was not loaded");
}

public record RecipeSectionResponse
{
    public required Ulid? RecipeSectionId { get; init; }
    public required string RecipeSectionName { get; init; }
    public required int SortOrder { get; init; }
    public required ReadOnlyList<RecipeSectionEntryResponse> RecipeSectionEntries { get; init; }
}

public record RecipeSectionEditRequest
{
    [StringLength(255, MinimumLength = 1)] 
    public required string RecipeSectionName { get; init; }
    public required int SortOrder { get; init; }
}