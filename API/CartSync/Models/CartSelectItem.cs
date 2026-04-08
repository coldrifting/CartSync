using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Text.Json.Serialization;
using CartSync.Controllers.Core;
using CartSync.Objects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Models;

[PrimaryKey(nameof(CartSelectItemId))]
public class CartSelectItem
{
    public Ulid CartSelectItemId { get; init; } = Ulid.NewUlid();
    
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
    public static Expression<Func<CartSelectItem, CartSelectItemResponse>> ToResponse =>
        cartSelectItem => new CartSelectItemResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = cartSelectItem.ItemId,
                Name = cartSelectItem.Item.ItemName,
                Temp = cartSelectItem.Item.Temp,
            },
            Prep = (cartSelectItem.PrepId == null
                ? null
                : new PrepResponse
                {
                    Id = cartSelectItem.PrepId.Value,
                    Name = cartSelectItem.Prep!.PrepName
                })!,
            Amount = cartSelectItem.Amounts.Amount
        };

    // Errors
    public static NotFound<Error> NotFound(Ulid itemId, Ulid? prepId) =>
        Error.NotFoundCompositeKey("CartSelectItem", itemId, "Item", prepId, "Prep");
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
    
    public static Expression<Func<CartSelectItem, CartSelectItemResponse>> FromCartSelectItem =>
        cartSelectItem => new CartSelectItemResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = cartSelectItem.ItemId,
                Name = cartSelectItem.Item.ItemName,
                Temp = cartSelectItem.Item.Temp,
            },
            Prep = (cartSelectItem.PrepId == null
                ? null
                : new PrepResponse
                {
                    Id = cartSelectItem.PrepId.Value,
                    Name = cartSelectItem.Prep!.PrepName
                })!,
            Amount = cartSelectItem.Amounts.Amount
        };
}

public record CartSelectRecipeResponse
{
    public required Ulid RecipeId { get; init; }
    public required string RecipeName { get; init; }
    public required int Quantity { get; init; }
    
    public static Expression<Func<Recipe, CartSelectRecipeResponse>> FromRecipe =>
        recipe => new CartSelectRecipeResponse
        {
            RecipeId = recipe.RecipeId,
            RecipeName = recipe.RecipeName,
            Quantity = recipe.CartQuantity
        };
}

public record CartItemEditRequest
{
    public Amount Amount { get; init; } = new();
}

public record CartRecipeEditRequest
{
    public int Quantity { get; init; }
}
