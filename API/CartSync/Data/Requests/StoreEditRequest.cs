using System.ComponentModel.DataAnnotations;

namespace CartSync.Data.Requests;

public record StoreEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public required string Name { get; init; }
}