using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database.Objects;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(ItemId))]
public class Item
{
    public Ulid ItemId { get; set; } = Ulid.NewUlid();
    
    [StringLength(256)]
    public string ItemName { get; set; } = "(Default)";

    public ItemTemp ItemTemp { get; set; } = ItemTemp.Ambient;
    public UnitType DefaultUnitType { get; set; } = UnitType.Count;

    public Amount CartAmount { get; set; } = Amount.None;
    
    // Navigation
    public List<Prep> Preps { get; set; } = [];
    public List<ItemPrep> ItemPreps { get; set; } = [];
    
    // Enforce max of 1 linked aisle per store
    public List<Aisle> Aisles { get; set; } = [];
}

public class ItemResponse
{
    public Ulid ItemId { get; set; }
    
    [StringLength(256)]
    public string ItemName { get; set; } = "(Default)";

    public ItemTemp ItemTemp { get; set; } = ItemTemp.Ambient;
    public UnitType DefaultUnitType { get; set; } = UnitType.Count;
    
    public Amount? CartAmount { get; set; }
    
    public List<PrepResponse> Preps { get; set; } = [];
}

public class ItemResponseNoPrep
{
    public Ulid ItemId { get; set; }
    
    [StringLength(256)]
    public string ItemName { get; set; } = "(Default)";

    public ItemTemp ItemTemp { get; set; } = ItemTemp.Ambient;
}

public class ItemAddRequest
{
    [Required] 
    [StringLength(256)] 
    public string ItemName { get; set; } = null!;
    
    public ItemTemp? ItemTemp { get; set; } = null;
    public UnitType? DefaultUnitType { get; set; } = null;
}

public class ItemEditRequest
{
    [StringLength(256)]
    public string? ItemName { get; set; } = null;
    
    public ItemTemp? ItemTemp { get; set; } = null;
    public UnitType? DefaultUnitType { get; set; } = null;
    
    public List<Ulid>? PrepIds { get; set; } = null;
    
    public Amount? CartAmount { get; set; } = null;
}

public class ItemAisleResponse
{
    public AisleResponse?  Aisle { get; set; }
    public List<ItemResponse> Items { get; set; } = [];
}