using System.ComponentModel.DataAnnotations.Schema;
using CartSync.Data.Responses;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Data.Entities;

[PrimaryKey(nameof(CartSelectItemId))]
public class CartSelectItem
{
    public Ulid CartSelectItemId { get; init; } = Ulid.NewUlid();
    
    public Ulid ItemId { get; init; }
    public Ulid? PrepId { get; init; }

    public AmountGroup Amounts { get; set; } = new();
    
    // Navigation
    [ForeignKey(nameof(ItemId))]
    public Item Item { init; get => field ?? throw Item.NotLoaded; }
    
    [ForeignKey(nameof(PrepId))]
    public Prep? Prep { get; init; }

    // Errors
    public static NotFound<ErrorResponse> NotFound(Ulid itemId, Ulid? prepId) =>
        ErrorResponse.NotFoundCompositeKey("CartSelectItem", itemId, "Item", prepId, "Prep");
}