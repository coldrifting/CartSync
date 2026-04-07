using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(CartItemId))]
public class CartItem
{
    public Ulid CartItemId { get; init; } = Ulid.NewUlid();
    
    public Ulid ItemId { get; init; }
    public Ulid? PrepId { get; init; }

    public AmountGroup Amounts { get; set; } = new();
    
    // Navigation
    [JsonIgnore]
    [ForeignKey(nameof(ItemId))]
    public Item Item { init; get => field ?? throw Item.NotLoaded; }
    
    [JsonIgnore]
    [ForeignKey(nameof(PrepId))]
    public Prep? Prep { get; init; }
    
    // Projections
    public static Expression<Func<CartItem, CartSelectItemResponse>> ToResponse =>
        cartItem => new CartSelectItemResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = cartItem.ItemId,
                Name = cartItem.Item.ItemName,
                Temp = cartItem.Item.Temp,
            },
            Prep = (cartItem.PrepId == null
                ? null
                : new PrepResponse
                {
                    Id = cartItem.PrepId.Value,
                    Name = cartItem.Prep!.PrepName
                })!,
            Amount = cartItem.Amounts.Amount
        };

    // Errors
    public static NotFound<Error> NotFound(Ulid itemId, Ulid? prepId) =>
        Error.NotFoundCompositeKey("CartItem", itemId, "Item", prepId, "Prep");
}

public record CartInfo
{
    public Ulid ItemId { get; init; }
    public Ulid? PrepId { get; init; }
    public required AmountGroup Amounts { get; init; }
    public required bool UncapUnits { get; init; }
    
    public static Expression<Func<CartItem, CartInfo>> FromCartItem =>
        cartItem => new CartInfo
        {
            ItemId = cartItem.ItemId,
            PrepId = cartItem.PrepId,
            Amounts = cartItem.Amounts,
            UncapUnits = cartItem.Item.UncapCartUnits
        };
}

public record CartSelectResponse
{
    public CartSelectItemResponse[] Items { get; init; } = [];
    public CartSelectRecipeResponse[] Recipes { get; init; } = [];
}

public record CartSelectItemResponse
{
    public required ItemMinimalResponse Item { get; init; }
    public PrepResponse? Prep { get; init; }
    public Amount Amount { get; init; } = new();
}

public record CartSelectRecipeResponse
{
    public required Ulid RecipeId { get; init; }
    public required string RecipeName { get; init; }
    public required int Quantity { get; init; }
}

public record CartItemEditRequest
{
    public Amount Amount { get; init; } = new();
}

public record CartRecipeEditRequest
{
    public int Quantity { get; init; }
}
