using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Misc;

public record CartEntryPrototype
{
    public required Ulid ItemId { get; init; }
    public required Ulid? PrepId { get; init; }
    public required AmountGroup Amounts { get; init; }
    public required bool UncapUnits { get; init; }

    public static Expression<Func<CartSelectItem, CartEntryPrototype>> FromCartItem =>
        cartItem => new CartEntryPrototype
        {
            ItemId = cartItem.ItemId,
            PrepId = cartItem.PrepId,
            Amounts = cartItem.Amounts,
            UncapUnits = cartItem.Item.UncapCartUnits
        };

    public static Expression<Func<RecipeEntry, CartEntryPrototype>> FromRecipeEntry =>
        recipeEntry => new CartEntryPrototype
        {
            ItemId = recipeEntry.ItemId,
            PrepId = recipeEntry.PrepId,
            Amounts = recipeEntry.Amount.Multiply(recipeEntry.RecipeSection.Recipe.CartQuantity,
                recipeEntry.Item.UncapCartUnits),
            UncapUnits = recipeEntry.Item.UncapCartUnits
        };

    public static Func<IGrouping<ItemPrepPair, CartEntryPrototype>, CartEntryPrototype> Aggregate =>
        cartEntryGroup => new CartEntryPrototype
        {
            ItemId = cartEntryGroup.Key.ItemId,
            PrepId = cartEntryGroup.Key.PrepId,
            Amounts = cartEntryGroup.Aggregate(AmountGroup.None,
                (totalAmount, cartInfo) => cartInfo.Amounts.Add(totalAmount, cartEntryGroup.First().UncapUnits)),
            UncapUnits = cartEntryGroup.First().UncapUnits
        };
}