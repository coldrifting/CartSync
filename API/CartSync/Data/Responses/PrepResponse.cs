using System.Linq.Expressions;
using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record PrepResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    
    public static Expression<Func<Prep, PrepResponse>> FromEntity =>
        prep => new PrepResponse
        {
            Id = prep.PrepId,
            Name = prep.PrepName
        };

    public static PrepResponse FromNewEntity(Prep prep) => FromEntity.Compile()(prep);
}