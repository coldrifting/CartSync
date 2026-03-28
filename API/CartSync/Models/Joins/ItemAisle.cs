using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using CartSync.Objects.Enums;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models.Joins;

// Maps an item to a location in a store
[PrimaryKey(nameof(ItemId), nameof(StoreId))]
public class ItemAisle
{
    public Ulid ItemId { get; init; }
    public Ulid StoreId { get; init; }
    
    public Ulid AisleId { get; set; }
    public BayType Bay { get; set; } = BayType.Middle;
    
    // Navigation
    [ForeignKey(nameof(ItemId))]
    public Item Item { init; get => field ?? throw Item.NotLoaded; }
    
    [ForeignKey(nameof(StoreId))]
    public Store Store { init; get => field ?? throw Store.NotLoaded; }

    [ForeignKey(nameof(AisleId))]
    public Aisle Aisle { set; get => field ?? throw Aisle.NotLoaded; }
    
    // Projections
    public static Expression<Func<ItemAisle, ItemAisleResponse>> ToResponse =>
        itemAisle => new ItemAisleResponse
        {
            AisleId = itemAisle.AisleId,
            StoreId = itemAisle.StoreId,
            AisleName = itemAisle.Aisle.AisleName,
            SortOrder = itemAisle.Aisle.SortOrder,
            Bay = itemAisle.Bay
        };

    public ItemAisleEditRequest ToEditRequest()
    {
        return new ItemAisleEditRequest
        {
            AisleId = AisleId,
            Bay = Bay,
        };
    }
}

public record ItemAisleResponse
{
    public required Ulid AisleId { get; init; }
    public required Ulid StoreId { get; init; }
    public required string AisleName { get; init; }
    public required BayType Bay { get; init; }
    public required int SortOrder { get; init; }
}

public record ItemAisleEditRequest
{
    [Required]
    public required Ulid AisleId { get; init; }
    
    [Required]
    public required BayType Bay { get; init; }
}