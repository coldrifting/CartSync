using System.ComponentModel.DataAnnotations;

namespace CartSync.Data.Requests;

public record RecipeSectionEditRequest
{
    [StringLength(255, MinimumLength = 1)] 
    public required string Name { get; init; }
    public required int SortOrder { get; init; }
}