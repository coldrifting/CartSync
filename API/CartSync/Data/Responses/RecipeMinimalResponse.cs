using System.Linq.Expressions;
using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record RecipeMinimalResponse
{
    public required Ulid Id { get; init; }
    
    public required string Name { get; init; }
    public required bool IsPinned { get; init; }

    public static Expression<Func<Recipe, RecipeMinimalResponse>> FromEntity =>
        recipe => new RecipeMinimalResponse
        {
            Id = recipe.RecipeId,
            Name = recipe.RecipeName,
            IsPinned = recipe.IsPinned
        };

    public static RecipeMinimalResponse FromNewEntity(Recipe recipe) => FromEntity.Compile()(recipe);
}