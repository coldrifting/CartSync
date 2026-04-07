using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using CartSync.Controllers.Core;
using CartSync.Models.Interfaces;
using CartSync.Utils;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(AisleId))]
public class Aisle : ISortable, IEditable<AisleEditRequest>, IResponse<Aisle, AisleResponse>
{
    public Ulid AisleId { get; init; } = Ulid.NewUlid();
    public Ulid StoreId { get; init; }
    
    [StringLength(256, MinimumLength = 1)]
    public string AisleName { get; set; } = "(Default)";
    public int SortOrder { get; set; } = -1;
    
    // Navigation
    [ForeignKey(nameof(StoreId))]
    public Store Store { init; get => field ?? throw Store.NotLoaded; }

    public List<Item> Items { get; init; } = [];
    
    // Projections
    public static Expression<Func<Aisle, AisleResponse>> ToResponse =>
        aisle => new AisleResponse
        {
            Id = aisle.AisleId,
            Name = aisle.AisleName,
            SortOrder = aisle.SortOrder
        };
    
    public AisleResponse ToNewResponse =>
        ToResponse.Compile()(this);
    
    // Conversion and Validation
    public AisleEditRequest ToEditRequest(Ulid? storeId = null)
    {
        return new AisleEditRequest
        {
            Name = AisleName,
            SortOrder = SortOrder
        };
    }

    /// Requires Aisle.Store.Aisles navigation to work
    public void UpdateFromEditRequest(AisleEditRequest editRequest)
    {
        AisleName = editRequest.Name;
        
        int oldIndex = SortOrder;
        Store.Aisles.Reorder(oldIndex, editRequest.SortOrder);
    }

    // Errors
    public static NotFound<Error> NotFound(Ulid aisleId) =>
        Error.NotFound(aisleId, "Aisle");
    
    public static NotFound<Error> NotFoundUnderStore(Ulid aisleId, Ulid storeId) =>
        Error.NotFoundUnder(aisleId, "Aisle", storeId, "Store");
    
    public static InvalidOperationException NotLoaded => new("Aisle Navigation Property was not loaded");
}

public record AisleResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required int SortOrder { get; init; }
}

public record AisleAddRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public required string Name { get; init; }
}

public record AisleEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public required string Name { get; init; }
    
    [Required]
    public required int SortOrder { get; init; }
}