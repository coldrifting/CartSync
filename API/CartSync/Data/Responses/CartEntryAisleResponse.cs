using System.Linq.Expressions;
using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record CartEntryAisleResponse
{
    public required Ulid? AisleId { get; init; }
    public required string? AisleName { get; init; }
    public required CartEntryItemResponse[] Items { get; init; }
    
    public static Expression<Func<IGrouping<Ulid?, CartEntry>, CartEntryAisleResponse>> FromAisleGroup =>
        aisleGroup => new CartEntryAisleResponse
        {
            AisleId = aisleGroup.Key,
            AisleName = aisleGroup.FirstOrDefault() != null ? aisleGroup.FirstOrDefault()!.Aisle!.AisleName : null,
            Items = aisleGroup.Select(ce => new CartEntryItemResponse
                {
                    Item = new ItemMinimalResponse
                    {
                        Id = ce.ItemId,
                        Name = ce.Item.ItemName,
                        Temp = ce.Item.Temp,
                        DefaultUnitType = ce.Item.DefaultUnitType,
                    },
                    Prep = ce.Prep != null ? PrepResponse.FromEntity.Compile()(ce.Prep) : null,
                    Bay = ce.Bay,
                    Amounts = ce.Amounts
                })
                .OrderBy(cir => cir.Bay)
                .ThenBy(cir => cir.Item.Name)
                .ThenBy(cir => cir.Item.Id)
                .ThenBy(cir => cir.Prep != null ? cir.Prep.Name : "")
                .ThenBy(cir => cir.Prep != null ? cir.Prep.Id : Ulid.Empty)
                .ToArray()
        };
}