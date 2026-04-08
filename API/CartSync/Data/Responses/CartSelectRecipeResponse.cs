using System.Linq.Expressions;
using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record CartSelectRecipeResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required int Quantity { get; init; }
    
    public static Expression<Func<Recipe, CartSelectRecipeResponse>> FromEntity =>
        recipe => new CartSelectRecipeResponse
        {
            Id = recipe.RecipeId,
            Name = recipe.RecipeName,
            Quantity = recipe.CartQuantity
        };
}