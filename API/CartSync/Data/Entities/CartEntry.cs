using System.ComponentModel.DataAnnotations.Schema;
using CartSync.Data.Responses;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(CartEntryId))]
public record CartEntry
{
    public Ulid CartEntryId { get; init; } = Ulid.NewUlid();
    
    public Ulid ItemId { get; init; }
    public Ulid? PrepId { get; init; }

    public required AmountGroup Amounts { get; init; }
    
    public Ulid? AisleId { get; init; }
    public Bay Bay { get; init; }

    public bool IsChecked { get; set; }

    // Navigation
    [ForeignKey(nameof(ItemId))]
    public Item Item { init; get => field ?? throw Item.NotLoaded; }
    
    [ForeignKey(nameof(PrepId))]
    public Prep? Prep { get; init; }
    
    [ForeignKey(nameof(AisleId))]
    public Aisle? Aisle { get; init; }

    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid itemId, Ulid? prepId) =>
        ErrorResponse.NotFoundCompositeKey("CartEntry", itemId, "Item", prepId, "Prep");
}