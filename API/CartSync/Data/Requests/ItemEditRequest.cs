using System.ComponentModel.DataAnnotations;
using CartSync.Objects;

namespace CartSync.Data.Requests;

public record ItemEditRequest
{
    [Required, StringLength(256, MinimumLength = 1)]
    public required string Name { get; init; }
    
    [Required] public required Temp Temp { get; init; }
    [Required] public required UnitType DefaultUnitType { get; init; }
    [Required] public required bool UncapCartUnits { get; init; }
    
    // Dont use required attribute
    public required LocationEditRequest? Location { get; init; }
}