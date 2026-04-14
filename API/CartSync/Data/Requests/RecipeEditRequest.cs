using System.ComponentModel.DataAnnotations;

namespace CartSync.Data.Requests;

public record RecipeEditRequest
{
    [Required, StringLength(255, MinimumLength = 1)]
    public required string Name  { get; init; }
    
    [Required, StringLength(2048)]
    public required string Url { get; init; }
    
    [Required] public required bool IsPinned { get; init; }
}