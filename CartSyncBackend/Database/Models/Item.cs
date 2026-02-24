using System.ComponentModel.DataAnnotations;
using CartSyncBackend.Database.Objects;

namespace CartSyncBackend.Database.Models;

public class Item
{
    [Key] 
    public Ulid ItemId { get; set; } = Ulid.NewUlid();
    
    [StringLength(256)]
    public string ItemName { get; set; } = "(Default)";

    public ItemTemp ItemTemp { get; set; } = ItemTemp.Ambient;
    public UnitType DefaultUnitType { get; set; } = UnitType.Count;
    
    public Amount? CartAmount { get; set; }

    public ItemResponse ToResponse()
    {
        return new ItemResponse()
        {
            ItemId = ItemId,
            ItemName = ItemName,
            ItemTemp = ItemTemp,
            DefaultUnitType = DefaultUnitType,
            CartAmount = CartAmount
        };
    }
}

public class ItemResponse
{
    public Ulid ItemId { get; set; }
    
    [StringLength(256)]
    public string ItemName { get; set; } = "(Default)";

    public ItemTemp ItemTemp { get; set; } = ItemTemp.Ambient;
    public UnitType DefaultUnitType { get; set; } = UnitType.Count;
    
    public Amount? CartAmount { get; set; }
}

public class ItemAddRequest
{
    [Required]
    [StringLength(256)]
    public string ItemName { get; set; } = "New Item";
    
    public ItemTemp? ItemTemp { get; set; } = null;
    public UnitType? DefaultUnitType { get; set; } = null;
}

public class ItemEditRequest
{
    [StringLength(256)]
    public string? ItemName { get; set; } = null;
    
    public ItemTemp? ItemTemp { get; set; } = null;
    public UnitType? DefaultUnitType { get; set; } = null;
}