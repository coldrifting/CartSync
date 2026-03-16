using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CartSyncBackend.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(AisleId))]
public class Aisle : ISortable
{
    public Ulid AisleId { get; init; } = Ulid.NewUlid();
    public Ulid StoreId { get; init; }
    
    [StringLength(256)]
    public string AisleName { get; set; } = "(Default)";
    public int SortOrder { get; set; } = -1;
    
    // Navigation
    [ForeignKey(nameof(StoreId))]
    public Store Store
    {
        set;
        get => field ?? throw new InvalidOperationException("Uninitialized property: " + nameof(Store));
    }

    public List<Item> Items { get; set; } = [];
}

public class AisleResponse
{
    public Ulid AisleId { get; set; }
    public string AisleName { get; set; } = string.Empty;
    public int SortOrder { get; set; } = -1;
}


public class AisleAddRequest
{
    [Required] 
    public Ulid StoreId { get; set; }
    
    [Required] 
    public string AisleName { get; set; } = string.Empty;
}

public class AisleEditRequest
{
    [Required] 
    public Ulid AisleId { get; set; }
    
    public string? AisleName { get; set; }
    public int? SortOrder { get; set; }
}