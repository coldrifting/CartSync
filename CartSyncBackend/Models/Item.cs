using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSyncBackend.Controllers.Core;
using CartSyncBackend.Models.Interfaces;
using CartSyncBackend.Models.Joins;
using CartSyncBackend.Objects;
using CartSyncBackend.Objects.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Models;

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
    public List<RecipeSectionEntry> RecipeSectionEntries { get; init; } = [];
    
    // Enforce max of 1 linked aisle per store
    public List<Aisle> Aisles { get; init; } = [];
    
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
                .Select(Prep.ToResponse)
                .ToImmutableList()
                .WithValueSemantics(),
            Locations = item.Aisles
                .AsQueryable()
                .OrderBy(a => a.Store.StoreName)
                .ThenBy(a => a.AisleName)
                .Select(Aisle.ToResponse)
                .ToImmutableList()
                .WithValueSemantics()
        };
    
    public static Expression<Func<Item, ItemResponse>> ToLocatedResponse(Ulid storeId) =>
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
            Locations = item.Aisles
                .AsQueryable()
                .OrderBy(a => a.Store.StoreName)
                .ThenBy(a => a.AisleName)
                .Where(a => a.StoreId == storeId)
                .Select(Aisle.ToResponse)
                .ToImmutableList()
                .WithValueSemantics()
        };

    public static Expression<Func<Item, ItemMinimalResponse>> ToMinimalResponse =>
        item => new ItemMinimalResponse
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemTemp = item.ItemTemp
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
            AisleId = Aisles.FirstOrDefault(a => a.StoreId == storeId)?.AisleId,
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
    public required ReadOnlyList<AisleResponse> Locations { get; init; }
}

public record ItemMinimalResponse
{
    public required Ulid ItemId { get; init; }
    public required string ItemName { get; init; }
    public required ItemTemp ItemTemp { get; init; }
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
    public required Ulid? AisleId { get; init; }
}