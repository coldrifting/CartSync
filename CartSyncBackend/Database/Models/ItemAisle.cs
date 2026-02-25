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
    public Item Item { get; set; } = null!;
    
    [ForeignKey(nameof(StoreId))]
    public Store Store { get; set; } = null!;

    [ForeignKey(nameof(AisleId))] 
    public Aisle Aisle { get; set; } = null!;
}

public class ItemAisleLocChangeRequest
{
    [Required]
    public Ulid AisleId { get; init; }
    public BayType? Bay { get; set; } = null;
}