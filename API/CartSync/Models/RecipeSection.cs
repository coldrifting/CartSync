using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using CartSync.Objects;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

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

    public List<RecipeEntry> Entries { get; init; } = [];
    
    // Projections
    public static Expression<Func<RecipeSection, RecipeSectionResponse>> ToResponse =>
        recipeSection => new RecipeSectionResponse
        {
            Id = recipeSection.RecipeSectionId,
            Name = recipeSection.RecipeSectionName,
            SortOrder = recipeSection.SortOrder,
            Entries = recipeSection.Entries
                .AsQueryable()
                .OrderBy(rse => rse.Item.Temp)
                .ThenBy(rse => rse.Item.ItemName)
                .ThenBy(rse => rse.Item.ItemId)
                .Select(RecipeEntry.ToResponse)
                .ToImmutableList()
                .WithValueSemantics()
        };
    
    public RecipeSectionResponse ToNewResponse =>
        new()
        {
            Id = RecipeSectionId,
            Name = RecipeSectionName,
            SortOrder = SortOrder,
            Entries = []
        };

    
    // Conversion and Validation
    public RecipeSectionEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new RecipeSectionEditRequest
        {
            Name = RecipeSectionName,
            SortOrder = SortOrder
        };
    }
    
    /// Requires RecipeSection.Recipe.RecipeSections navigation to work
    public void UpdateFromEditRequest(RecipeSectionEditRequest editRequest)
    {
        RecipeSectionName = editRequest.Name;
        
        int oldIndex = SortOrder;
        Recipe.Sections.Reorder(oldIndex, editRequest.SortOrder);
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
    public required Ulid? Id { get; init; }
    public required string Name { get; init; }
    public required int SortOrder { get; init; }
    public required ReadOnlyList<RecipeEntryResponse> Entries { get; init; }
}

public record RecipeSectionAddRequest
{
    [StringLength(255, MinimumLength = 1)] 
    public required string Name { get; init; }
}

public record RecipeSectionEditRequest
{
    [StringLength(255, MinimumLength = 1)] 
    public required string Name { get; init; }
    public required int SortOrder { get; init; }
}