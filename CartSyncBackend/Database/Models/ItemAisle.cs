using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CartSyncBackend.Database.Objects;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

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
    public Item Item
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Item));
    }
    
    [ForeignKey(nameof(StoreId))]
    public Store Store
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Store));
    }

    [ForeignKey(nameof(AisleId))] 
    public Aisle Aisle
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Aisle));
    }
}

public class ItemAisleLocChangeRequest
{
    [Required]
    public Ulid AisleId { get; init; }
    public BayType? Bay { get; set; } = null;
}