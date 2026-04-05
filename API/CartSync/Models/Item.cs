using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using CartSync.Models.Joins;
using CartSync.Objects;
using CartSync.Objects.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(ItemId))]
public class Item : IEditable<ItemEditRequest>, IResponse<Item, ItemResponse>
{
    public Ulid ItemId { get; init; } = Ulid.NewUlid();
    
    [StringLength(256, MinimumLength = 1)]
    public required string ItemName { get; set; }

    public ItemTemp ItemTemp { get; set; }
    public UnitType DefaultUnitType { get; set; }
    
    // Navigation
    public List<Prep> Preps { get; init; } = [];
    public List<ItemPrep> ItemPreps { get; init; } = [];
    public List<RecipeEntry> RecipeSectionEntries { get; init; } = [];
    
    // Enforce max of 1 linked aisle per store
    public List<Aisle> Aisles { get; init; } = [];
    public List<ItemAisle> ItemAisles { get; init; } = [];
    
    // Projections
    public static Expression<Func<Item, ItemResponse>> ToResponse =>
        item => new ItemResponse
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemTemp = item.ItemTemp,
            DefaultUnitType = item.DefaultUnitType,
            Preps = item.Preps
                .AsQueryable()
                .OrderBy(p => p.PrepName)
                .ThenBy(p => p.PrepId)
                .Select(Prep.ToResponse)
                .ToImmutableList()
                .WithValueSemantics(),
            Locations = item.ItemAisles
                .AsQueryable()
                .OrderBy(a => a.Store.StoreName)
                .ThenBy(a => a.Aisle.AisleName)
                .Select(ItemAisle.ToResponse)
                .ToImmutableList()
                .WithValueSemantics()
        };
    
    public static Expression<Func<Item, ItemByStoreResponse>> ToByStoreResponse(Ulid storeId) =>
        item => new ItemByStoreResponse
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemTemp = item.ItemTemp,
            DefaultUnitType = item.DefaultUnitType,
            Preps = item.Preps
                .AsQueryable()
                .OrderBy(p => p.PrepName)
                .ThenBy(p => p.PrepId)
                .Select(Prep.ToResponse)
                .ToImmutableList()
                .WithValueSemantics(),
            Location = item.ItemAisles
                .AsQueryable()
                .Select(ItemAisle.ToResponse)
                .FirstOrDefault(a => a.StoreId == storeId)
        };

    public static Expression<Func<Item, ItemMinimalResponse>> ToMinimalResponse =>
        item => new ItemMinimalResponse
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemTemp = item.ItemTemp
        };

    public static Expression<Func<Item, ItemUsagesResponse>> ToUsagesResponse =>
        item => new ItemUsagesResponse
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            Preps = item.Preps
                .AsQueryable()
                .OrderBy(p => p.PrepName)
                .ThenBy(p => p.PrepId)
                .Select(Prep.ToResponse)
                .ToImmutableList()
                .WithValueSemantics(),
            Recipes = item.RecipeSectionEntries
                .AsQueryable()
                .Select(r => r.RecipeSection.Recipe)
                .Distinct()
                .OrderBy(r => r.RecipeName)
                .ThenBy(r => r.RecipeId)
                .Select(Recipe.ToMinimalResponse)
                .ToImmutableList()
                .WithValueSemantics()
        };
    
    // Since new objects will have no children we can skip querying the db after inserting
    public ItemResponse ToNewResponse =>
        new()
        {
            ItemId = ItemId,
            ItemName = ItemName,
            ItemTemp = ItemTemp,
            DefaultUnitType = DefaultUnitType,
            Preps = [],
            Locations = []
        };
    
    // Conversion and Validation
    public ItemEditRequest ToEditRequest(Ulid? storeId)
    {
        return new ItemEditRequest
        {
            ItemName = ItemName,
            ItemTemp = ItemTemp,
            Location = ItemAisles.FirstOrDefault(a => a.StoreId == storeId)?.ToEditRequest(),
            DefaultUnitType = DefaultUnitType,
            PrepIds = Preps
                .OrderBy(p => p.PrepName)
                .ThenBy(p => p.PrepId)
                .Select(p => p.PrepId)
                .ToList()
        };
    }

    /// Requires Item.Preps Navigation to work
    public void UpdateFromEditRequest(ItemEditRequest editRequest)
    {
        ItemName = editRequest.ItemName;
        ItemTemp = editRequest.ItemTemp;
        DefaultUnitType = editRequest.DefaultUnitType;

        ItemPreps.Clear();

        foreach (Ulid prepId in editRequest.PrepIds.ToHashSet())
        {
            ItemPreps.Add(new ItemPrep
            {
                ItemId = ItemId,
                PrepId = prepId
            });
        }
    }

    // Errors
    public static NotFound<Error> NotFound(Ulid itemId) => 
        Error.NotFound(itemId, "Item");
    
    public static InvalidOperationException NotLoaded => new("Item Navigation Property was not loaded");
}

public record ItemResponse
{
    public required Ulid ItemId { get; init; }
    public required string ItemName { get; init; }
    public required ItemTemp ItemTemp { get; init; }
    public required UnitType DefaultUnitType { get; init; }
    public required ReadOnlyList<PrepResponse> Preps { get; init; }
    public required ReadOnlyList<ItemAisleResponse> Locations { get; init; }
}

public record ItemByStoreResponse
{
    public required Ulid ItemId { get; init; }
    public required string ItemName { get; init; }
    public required ItemTemp ItemTemp { get; init; }
    public required UnitType DefaultUnitType { get; init; }
    public required ReadOnlyList<PrepResponse> Preps { get; init; }
    public required ItemAisleResponse? Location { get; init; }
}

public record ItemMinimalResponse
{
    public required Ulid ItemId { get; init; }
    public required string ItemName { get; init; }
    public required ItemTemp ItemTemp { get; init; }
}

public record ItemUsagesResponse
{
    public required Ulid ItemId { get; init; }
    public required string ItemName { get; init; }
    public required ReadOnlyList<PrepResponse> Preps { get; init; }
    public required ReadOnlyList<RecipeMinimalResponse> Recipes { get; init; }
}

public record ItemAddRequest
{
    [StringLength(256, MinimumLength = 1)]
    public required string ItemName { get; init; }
}

public record ItemEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public required string ItemName { get; init; }
    
    [Required] public required ItemTemp ItemTemp { get; init; }
    [Required] public required UnitType DefaultUnitType { get; init; }
    [Required] public required List<Ulid> PrepIds { get; init; }
    
    // Dont use required attribute
    public required ItemAisleEditRequest? Location { get; init; }
}