using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record ItemWithPrepsResponse
{
    public required ItemMinimalResponse Item { get; init; }
    public required ReadOnlyList<PrepResponse?> Preps { get; init; }
    public required bool HasExtraPreps { get; init; }
        
    public static Func<Item, ItemWithPrepsResponse> FromEntity =>
        item => new ItemWithPrepsResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = item.ItemId,
                Name = item.ItemName,
                DefaultUnitType = item.DefaultUnitType,
                Temp = item.Temp
            },
            Preps = item.Preps
                .Select(prep => new PrepResponse
                {
                    Id = prep.PrepId,
                    Name = prep.PrepName,
                })
                .OrderBy(prep => prep.Name)
                .ThenBy(prep => prep.Id)
                .Prepend(null)
                .ToReadOnlyList(),
            HasExtraPreps = item.Preps.Count > 0
        };
}