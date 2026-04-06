using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Objects;
using CartSync.Objects.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(CartEntryId))]
public class CartEntry
{
    public Ulid CartEntryId { get; init; } = Ulid.NewUlid();
    
    public Ulid ItemId { get; init; }
    public Ulid? PrepId { get; init; }

    public required Amount Amount { get; init; }
    
    public Ulid? AisleId { get; init; }
    public BayType Bay { get; init; }

    public bool IsChecked { get; set; } = false;

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

public record CartResponse
{
    public Ulid StoreId { get; init; }
    public string StoreName { get; init; } = "";
    public CartAisleResponse[] Aisles { get; init; } = [];
}

public record CartAisleResponse
{
    public Ulid? AisleId { get; init; }
    public string? AisleName { get; init; } = "";
    public CartItemResponse[] Items { get; init; } = [];
}

public record CartItemResponse
{
    public required ItemMinimalResponse Item;
    public Prep? Prep { get; init; }
    public BayType Bay { get; init; } = BayType.Middle;
    public Amount Amount { get; init; } = new();
}