using System.ComponentModel.DataAnnotations;

namespace CartSync.Data.Requests;

public class ItemPrepEditRequest
{
    [Required] public required List<Ulid> PrepIds { get; init; }
}