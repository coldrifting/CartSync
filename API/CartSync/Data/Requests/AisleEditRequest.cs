using System.ComponentModel.DataAnnotations;

namespace CartSync.Data.Requests;

public record AisleEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public required string Name { get; init; }
    
    [Required]
    public required int SortOrder { get; init; }
}