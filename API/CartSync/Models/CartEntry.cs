using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Objects;
using CartSync.Objects.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

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
    [JsonIgnore]
    [ForeignKey(nameof(ItemId))]
    public Item Item { init; get => field ?? throw Item.NotLoaded; }
    
    [JsonIgnore]
    [ForeignKey(nameof(PrepId))]
    public Prep? Prep { get; init; }
    
    [JsonIgnore]
    [ForeignKey(nameof(AisleId))]
    public Aisle? Aisle { get; init; }

    // Errors
    public static NotFound<Error> NotFound(Ulid itemId, Ulid? prepId) =>
        Error.NotFoundCompositeKey("CartEntry", itemId, "Item", prepId, "Prep");
}

public record CartEntryValue
{
    public required Ulid ItemId { get; init; }
    public required Ulid? PrepId { get; init; }
    public required AmountGroup Amounts { get; init; }
    public required Ulid? AisleId { get; init; }
    public required Bay Bay { get; init; }
    public required bool IsChecked { get; init; }
    
    public static Expression<Func<CartEntry, CartEntryValue>> FromCartEntry =>
        cartEntry => new CartEntryValue
        {
            ItemId = cartEntry.ItemId,
            PrepId = cartEntry.PrepId,
            Amounts = cartEntry.Amounts,
            AisleId = cartEntry.AisleId,
            Bay = cartEntry.Bay,
            IsChecked = cartEntry.IsChecked
        };
}

public record CartResponse
{
    public required Ulid StoreId { get; init; }
    public required string StoreName { get; init; }
    public required CartAisleResponse[] Aisles { get; init; }
}

public record CartAisleResponse
{
    public required Ulid? AisleId { get; init; }
    public required string? AisleName { get; init; }
    public required CartItemResponse[] Items { get; init; }
}

public record CartItemResponse
{
    public required ItemMinimalResponse Item;
    public required Prep? Prep { get; init; }
    public required Bay Bay { get; init; }
    public required AmountGroup Amounts { get; init; }
}