using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using CartSyncBackend.Database.Interfaces;
using CartSyncBackend.Database.Objects;
using CartSyncBackend.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(AisleId))]
public class Aisle : ISortable, IEditable<AisleEditRequest>
{
    public Ulid AisleId { get; init; } = Ulid.NewUlid();
    public Ulid StoreId { get; init; }
    
    [StringLength(256, MinimumLength = 1)]
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
    
    // Projections
    public AisleResponse ToNewResponse =>
        new()
        {
            AisleId = AisleId,
            StoreId = StoreId,
            AisleName = AisleName,
            SortOrder = SortOrder
        };
    
    public static Expression<Func<Aisle, AisleResponse>> ToResponse =>
        aisle => new AisleResponse
        {
            AisleId = aisle.AisleId,
            StoreId = aisle.StoreId,
            AisleName = aisle.AisleName,
            SortOrder = aisle.SortOrder
        };
    
    // Conversion and Validation
    public AisleEditRequest ToEditRequest()
    {
        return new AisleEditRequest
        {
            AisleName = AisleName,
            SortOrder = SortOrder
        };
    }

    /// Requires Aisle.Store.Aisles navigation to work
    public void UpdateFromEditRequest(AisleEditRequest editRequest)
    {
        AisleName = editRequest.AisleName;
        
        int oldIndex = SortOrder;
        Store.Aisles.Reorder(oldIndex, editRequest.SortOrder);
        //SortOrder = editRequest.SortOrder;
    }

    // Errors
    public static NotFoundObjectResult NotFound(Ulid aisleId) =>
        Error.NotFound(aisleId, "Aisle");
    
    public static NotFoundObjectResult NotFoundUnderStore(Ulid aisleId, Ulid storeId) =>
        Error.NotFoundUnder(aisleId, "Aisle", storeId, "Store");
}

public class AisleResponse
{
    public Ulid AisleId { get; init; }
    public Ulid StoreId { get; init; }
    public string AisleName { get; init; } = string.Empty;
    public int SortOrder { get; init; } = -1;
    
    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not AisleResponse other)
        {
            return false;
        }

        return AisleId.Equals(other.AisleId) && 
               string.Equals(AisleName, other.AisleName, StringComparison.Ordinal) && 
               SortOrder.Equals(other.SortOrder) && 
               StoreId.Equals(other.StoreId);
    }

    public override int GetHashCode()
    {
        return AisleId.GetHashCode();
    }
}

public class AisleAddRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public string AisleName { get; init; } = string.Empty;
}

public class AisleEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public string AisleName { get; init; } = string.Empty;
    
    [Required]
    public int SortOrder { get; init; }
}