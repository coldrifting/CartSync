using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(StoreId))]
public class Store : IEditable<StoreEditRequest>, IResponse<Store, StoreResponse>
{
    public Ulid StoreId { get; init; } = Ulid.NewUlid();

    [StringLength(256, MinimumLength = 1)]
    public string StoreName { get; set; } = string.Empty;
    
    // Navigation
    public List<Aisle> Aisles { get; init; } = [];

    // Projections
    public StoreResponse ToNewResponse =>
        new()
        {
            Id = StoreId,
            Name = StoreName,
            IsSelected = false
        };
    
    public static Expression<Func<Store, StoreResponse>> ToResponse =>
        store => new StoreResponse
        {
            Id = store.StoreId,
            Name = store.StoreName,
            IsSelected = false
        };
    
    // Conversion and Validation
    public StoreEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new StoreEditRequest
        {
            Name = StoreName
        };
    }

    public void UpdateFromEditRequest(StoreEditRequest editRequest)
    {
        StoreName = editRequest.Name;
    }
    
    // Errors
    public static NotFound<Error> NotFound(Ulid storeId)
    {
        return Error.NotFound(storeId, "Store");
    }
    
    public static Conflict<Error> ConflictSelected(Ulid storeId)
    {
        return Error.Conflict($"Unable to delete store with id {storeId} while it is still the selected store");
    }

    public static InvalidOperationException NotLoaded => new("Store Navigation Property was not loaded");
}

public record StoreResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsSelected { get; init; }
}

public record StoreAddRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public required string Name { get; init; }
}

public record StoreEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public required string Name { get; init; }
}