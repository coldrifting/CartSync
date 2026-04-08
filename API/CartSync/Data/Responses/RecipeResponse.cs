using System.Collections.Immutable;
using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record RecipeResponse
{
    public required Ulid Id { get; init; }
    
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required bool IsPinned { get; init; }

    public required ReadOnlyList<RecipeStepResponse> Steps { get; init; }
    public required ReadOnlyList<RecipeSectionResponse> Sections { get; init; }
        
    public static Expression<Func<Recipe, RecipeResponse>> FromEntity =>
        recipe => new RecipeResponse
        {
            Id = recipe.RecipeId,
            Name = recipe.RecipeName,
            Url = recipe.Url,
            IsPinned = recipe.IsPinned,
            Steps = recipe.Steps
                .AsQueryable()
                .OrderBy(step => step.SortOrder)
                .Select(RecipeStepResponse.FromEntity)
                .ToImmutableList()
                .WithValueSemantics(),
            Sections = recipe.Sections
                .AsQueryable()
                .OrderBy(section => section.SortOrder)
                .Select(RecipeSectionResponse.FromEntity)
                .ToImmutableList()
                .WithValueSemantics(),
        };
}