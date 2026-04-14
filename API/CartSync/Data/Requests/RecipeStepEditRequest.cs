using System.ComponentModel.DataAnnotations;

namespace CartSync.Data.Requests;

public record RecipeStepEditRequest
{
    [StringLength(2048)]
    public required string Content { get; init; }
    public required bool IsImage { get; init; }
    public required int SortOrder { get; init; }
}