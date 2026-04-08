using System.Linq.Expressions;
using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record CartSelectRecipeResponse
{
    public required Ulid RecipeId { get; init; }
    public required string RecipeName { get; init; }
    public required int Quantity { get; init; }
    
    public static Expression<Func<Recipe, CartSelectRecipeResponse>> FromEntity =>
        recipe => new CartSelectRecipeResponse
        {
            RecipeId = recipe.RecipeId,
            RecipeName = recipe.RecipeName,
            Quantity = recipe.CartQuantity
        };
}