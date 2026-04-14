using System.ComponentModel.DataAnnotations.Schema;
using CartSync.Objects;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

// Maps an item to a location in a store
[PrimaryKey(nameof(ItemId), nameof(StoreId))]
public class ItemAisle
{
    public Ulid ItemId { get; init; }
    public Ulid StoreId { get; init; }
    
    public Ulid AisleId { get; set; }
    public Bay Bay { get; set; } = Bay.Center;
    
    // Navigation
    [ForeignKey(nameof(ItemId))]
    public Item Item { init; get => field ?? throw Item.NotLoaded; }
    
    [ForeignKey(nameof(StoreId))]
    public Store Store { init; get => field ?? throw Store.NotLoaded; }

    [ForeignKey(nameof(AisleId))]
    public Aisle Aisle { init; get => field ?? throw Aisle.NotLoaded; }
}



