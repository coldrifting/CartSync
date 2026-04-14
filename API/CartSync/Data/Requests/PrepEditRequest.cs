using System.ComponentModel.DataAnnotations;

namespace CartSync.Data.Requests;

public record PrepEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)] 
    public required string Name { get; init; }
}