using System.ComponentModel.DataAnnotations;

namespace CartSync.Data.Requests;

public record RecipeStepAddRequest
{
    [StringLength(2048)]
    public required string Content { get; init; } = string.Empty;
    public required bool IsImage { get; init; }
}