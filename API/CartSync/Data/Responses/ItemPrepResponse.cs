using System.Linq.Expressions;
using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record ItemPrepResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsSelected { get; init; }
    
    public static Expression<Func<Prep, ItemPrepResponse>> FromEntity(Item item) =>
        prep => new ItemPrepResponse
        {
            Id = prep.PrepId,
            Name = prep.PrepName,
            IsSelected = item.Preps.FirstOrDefault(p => p.PrepId == prep.PrepId) != null
        };
}