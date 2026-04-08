using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Requests;

public record LocationEditRequest
{
    [Required]
    public required Ulid AisleId { get; init; }
    
    [Required]
    public required Bay Bay { get; init; }
    
    public static Expression<Func<ItemAisle, LocationEditRequest>> FromItemAisle =>
        itemAisle => new LocationEditRequest
        {
            AisleId = itemAisle.AisleId,
            Bay = itemAisle.Bay
        };
}