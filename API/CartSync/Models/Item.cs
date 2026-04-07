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

    public Temp Temp { get; set; }
    public UnitType DefaultUnitType { get; set; }
    public bool UncapCartUnits { get; set; }

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
            Id = item.ItemId,
            Name = item.ItemName,
            Temp = item.Temp,
            DefaultUnitType = item.DefaultUnitType,
            UncapCartUnits = item.UncapCartUnits,
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
            Id = item.ItemId,
            Name = item.ItemName,
            Temp = item.Temp,
            DefaultUnitType = item.DefaultUnitType,
            UncapCartUnits = item.UncapCartUnits,
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
            Id = item.ItemId,
            Name = item.ItemName,
            Temp = item.Temp
        };

    public static Expression<Func<Item, ItemUsagesResponse>> ToUsagesResponse =>
        item => new ItemUsagesResponse
        {
            Id = item.ItemId,
            Name = item.ItemName,
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
            Id = ItemId,
            Name = ItemName,
            Temp = Temp,
            DefaultUnitType = DefaultUnitType,
            UncapCartUnits = UncapCartUnits,
            Preps = [],
            Locations = []
        };
    
    // Conversion and Validation
    public ItemEditRequest ToEditRequest(Ulid? storeId)
    {
        return new ItemEditRequest
        {
            Name = ItemName,
            Temp = Temp,
            Location = ItemAisles.FirstOrDefault(a => a.StoreId == storeId)?.ToEditRequest(),
            DefaultUnitType = DefaultUnitType,
            UncapCartUnits = UncapCartUnits,
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
        ItemName = editRequest.Name;
        Temp = editRequest.Temp;
        DefaultUnitType = editRequest.DefaultUnitType;
        UncapCartUnits = editRequest.UncapCartUnits;

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
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required Temp Temp { get; init; }
    public required UnitType DefaultUnitType { get; init; }
    public required bool UncapCartUnits { get; init; }
    public required ReadOnlyList<PrepResponse> Preps { get; init; }
    public required ReadOnlyList<ItemAisleResponse> Locations { get; init; }
}

public record ItemByStoreResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required Temp Temp { get; init; }
    public required UnitType DefaultUnitType { get; init; }
    public required bool UncapCartUnits { get; init; }
    public required ReadOnlyList<PrepResponse> Preps { get; init; }
    public required ItemAisleResponse? Location { get; init; }
}

public record ItemMinimalResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required Temp Temp { get; init; }
}

public record ItemUsagesResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required ReadOnlyList<PrepResponse> Preps { get; init; }
    public required ReadOnlyList<RecipeMinimalResponse> Recipes { get; init; }
}

public record ItemAddRequest
{
    [StringLength(256, MinimumLength = 1)]
    public required string Name { get; init; }
}

public record ItemEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public required string Name { get; init; }
    
    [Required] public required Temp Temp { get; init; }
    [Required] public required UnitType DefaultUnitType { get; init; }
    [Required] public required bool UncapCartUnits { get; init; }
    [Required] public required List<Ulid> PrepIds { get; init; }
    
    // Dont use required attribute
    public required ItemAisleEditRequest? Location { get; init; }
}