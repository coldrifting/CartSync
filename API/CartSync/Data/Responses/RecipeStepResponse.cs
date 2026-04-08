using System.Linq.Expressions;
using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record RecipeStepResponse
{
    public required Ulid Id { get; init; }
    public required string Content { get; init; }
    public required bool IsImage { get; init; }
    public required int SortOrder { get; init; }
    
    public static Expression<Func<RecipeStep, RecipeStepResponse>> FromEntity =>
        step => new RecipeStepResponse
        {
            Id = step.RecipeStepId,
            Content = step.RecipeStepContent,
            SortOrder = step.SortOrder,
            IsImage = step.IsImage
        };
    
    public static RecipeStepResponse FromNewEntity(RecipeStep step) => FromEntity.Compile()(step);
}