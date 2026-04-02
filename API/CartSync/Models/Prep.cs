using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using CartSync.Models.Joins;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(PrepId))]
public class Prep : IEditable<PrepEditRequest>, IResponse<Prep, PrepResponse>
{
    public Ulid PrepId { get; init; } = Ulid.NewUlid();
    
    [StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; set; }

    // Navigation
    public List<Item> Items { get; init; } = [];
    public List<ItemPrep> ItemPreps { get; init; } = [];
    public List<RecipeSectionEntry> RecipeSectionEntries { get; init; } = [];
    
    // Projections
    public static Expression<Func<Prep, PrepResponse>> ToResponse =>
        prep => new PrepResponse
        {
            PrepId = prep.PrepId,
            PrepName = prep.PrepName
        };
    
    public static Expression<Func<Prep, PrepUsagesResponse>> ToUsagesResponse =>
        prep => new PrepUsagesResponse
        {
            PrepId = prep.PrepId,
            PrepName = prep.PrepName,
            Items = prep.Items
                .AsQueryable()
                .OrderBy(i => i.ItemName)
                .ThenBy(i => i.ItemId)
                .Select(Item.ToMinimalResponse)
                .ToImmutableList()
                .WithValueSemantics(),
            Recipes = prep.RecipeSectionEntries
                .AsQueryable()
                .Select(r => r.RecipeSection.Recipe)
                .Distinct()
                .OrderBy(r => r.RecipeName)
                .ThenBy(r => r.RecipeId)
                .Select(Recipe.ToMinimalResponse)
                .ToImmutableList()
                .WithValueSemantics()
        };
    
    public PrepResponse ToNewResponse =>
        ToResponse.Compile()(this);

    // Conversion and Validation
    public PrepEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new PrepEditRequest
        {
            PrepName = PrepName
        };
    }

    public void UpdateFromEditRequest(PrepEditRequest editRequest)
    {
        PrepName = editRequest.PrepName;
    }
    
    // Errors
    public static NotFound<Error> NotFound(Ulid prepId) => 
        Error.NotFound(prepId, "Prep");
    
    public static NotFound<Error> NotFoundUnder(Ulid prepId, Ulid itemId) => 
        Error.NotFoundUnder(prepId, "Prep", itemId, "Item");
    
    public static InvalidOperationException NotLoaded => new("Prep Navigation Property was not loaded");
}

public record PrepResponse
{
    public required Ulid PrepId { get; init; }
    public required string PrepName { get; init; }
}


public record PrepUsagesResponse
{
    public required Ulid PrepId { get; init; }
    public required string PrepName { get; init; }
    public required ReadOnlyList<ItemMinimalResponse> Items { get; init; }
    public required ReadOnlyList<RecipeMinimalResponse> Recipes { get; init; }
}

public record PrepAddRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; init; }
}

public record PrepEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public required string PrepName { get; init; }
}