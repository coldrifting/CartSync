using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSyncBackend.Database.Interfaces;
using CartSyncBackend.Database.Objects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CartSyncBackend.Database.Models;

[PrimaryKey(nameof(StoreId))]
public class Store : IEditable<StoreEditRequest>
{
    public Ulid StoreId { get; init; } = Ulid.NewUlid();

    [StringLength(256, MinimumLength = 1)]
    public string StoreName { get; set; } = string.Empty;
    
    // Navigation
    public List<Aisle> Aisles { get; init; } = [];

    // Projections
    public static Expression<Func<Store, StoreResponse>> ToStoreResponse =>
        store => new StoreResponse
        {
            StoreId = store.StoreId,
            StoreName = store.StoreName
        };
    
    // Conversion and Validation
    public StoreEditRequest ToEditRequest()
    {
        return new StoreEditRequest
        {
            StoreName = StoreName
        };
    }

    public void UpdateFromEditRequest(StoreEditRequest editRequest)
    {
        StoreName = editRequest.StoreName;
    }
    
    // Errors
    public static NotFoundObjectResult NotFound(Ulid storeId) => 
        Error.NotFound(storeId, "Store");
}

public class StoreResponse
{
    public Ulid StoreId { get; init; }
    public required string StoreName { get; init; }
}

public class StoreAddRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public string StoreName { get; init; } = string.Empty;
}

public class StoreEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public string StoreName { get; init; } = string.Empty;
}