using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSyncBackend.Database.Interfaces;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(ItemId))]
public class Item : IEditable<ItemEditRequest>
{
    public Ulid ItemId { get; set; } = Ulid.NewUlid();
    
    [StringLength(256, MinimumLength = 1)]
    public string ItemName { get; set; } = "(Default)";

    public ItemTemp ItemTemp { get; set; } = ItemTemp.Ambient;
    public UnitType DefaultUnitType { get; set; } = UnitType.Count;

    public Amount CartAmount { get; set; } = Amount.None;
    
    // Navigation
    public List<Prep> Preps { get; set; } = [];
    public List<ItemPrep> ItemPreps { get; set; } = [];
    
    // Enforce max of 1 linked aisle per store
    public List<Aisle> Aisles { get; set; } = [];
    
    // Projections
    public static Expression<Func<Item, ItemResponse>> ToResponse =>
        item => new ItemResponse
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemTemp = item.ItemTemp,
            DefaultUnitType = item.DefaultUnitType,
            CartAmount = item.CartAmount,
            Preps = item.Preps
                .AsQueryable()
                .OrderBy(p => p.PrepName)
                .Select(Prep.ToResponse)
                .ToList(),
            Locations = item.Aisles
                .AsQueryable()
                .OrderBy(a => a.Store.StoreName)
                .ThenBy(a => a.AisleName)
                .Select(Aisle.ToResponse)
                .ToList()
        };
    
    public static Expression<Func<Item, ItemResponse>> ToLocatedResponse(Ulid storeId) =>
        item => new ItemResponse
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemTemp = item.ItemTemp,
            DefaultUnitType = item.DefaultUnitType,
            CartAmount = item.CartAmount,
            Preps = item.Preps
                .AsQueryable()
                .OrderBy(p => p.PrepName)
                .ThenBy(p => p.PrepId)
                .Select(Prep.ToResponse)
                .ToList(),
            Locations = item.Aisles
                .AsQueryable()
                .OrderBy(a => a.Store.StoreName)
                .ThenBy(a => a.AisleName)
                .Where(a => a.StoreId == storeId)
                .Select(Aisle.ToResponse)
                .ToList()
        };

    public static Expression<Func<Item, ItemMinimalResponse>> ToMinimalResponse =>
        item => new ItemMinimalResponse
        {
            ItemId = item.ItemId,
            ItemName = item.ItemName,
            ItemTemp = item.ItemTemp
        };
    
    // Conversion and Validation
    public ItemEditRequest ToEditRequest()
    {
        return new ItemEditRequest
        {
            ItemName = ItemName,
            ItemTemp = ItemTemp,
            DefaultUnitType = DefaultUnitType,
            PrepIds = Preps.Select(p => p.PrepId).ToList(),
            CartAmount = CartAmount
        };
    }

    /// Requires Item.Preps Navigation to work
    public void UpdateFromEditRequest(ItemEditRequest editRequest)
    {
        ItemName = editRequest.ItemName;
        ItemTemp = editRequest.ItemTemp;
        DefaultUnitType = editRequest.DefaultUnitType;
        CartAmount = editRequest.CartAmount;

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
    public static NotFoundObjectResult NotFound(Ulid itemId) => 
        Error.NotFound(itemId, "Item");
}

public class ItemResponse
{
    public Ulid ItemId { get; set; }
    public string ItemName { get; set; } = "(Default)";
    public ItemTemp ItemTemp { get; set; } = ItemTemp.Ambient;
    public UnitType DefaultUnitType { get; set; } = UnitType.Count;
    public Amount? CartAmount { get; set; }
    public List<PrepResponse> Preps { get; set; } = [];
    public List<AisleResponse> Locations { get; set; } = [];
}

public class ItemMinimalResponse
{
    public Ulid ItemId { get; set; }
    public string ItemName { get; set; } = "(Default)";
    public ItemTemp ItemTemp { get; set; } = ItemTemp.Ambient;
}

public class ItemAddRequest
{
    [StringLength(256, MinimumLength = 1)]
    public required string ItemName { get; set; } = null!;
    public ItemTemp? ItemTemp { get; set; } = null;
    public UnitType? DefaultUnitType { get; set; } = null;
}

public class ItemEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public required string ItemName { get; init; }
    
    [Required]
    public required ItemTemp ItemTemp { get; init; }
    
    [Required]
    public required UnitType DefaultUnitType { get; init; }
    
    [Required]
    public required Amount CartAmount { get; init; }
    
    [Required]
    public required List<Ulid> PrepIds { get; init; }
    
    [Required]
    public Ulid? AisleId { get; init; }
}