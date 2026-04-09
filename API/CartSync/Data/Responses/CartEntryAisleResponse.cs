using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record CartEntryAisleResponse
{
    public required Ulid? AisleId { get; init; }
    public required string? AisleName { get; init; }
    public required ReadOnlyList<CartEntryItemResponse> Items { get; init; }

    public static Func<CartEntryAisleResponsePrototype, CartEntryAisleResponse> FromPrototype =>
        aislePrototype => new CartEntryAisleResponse
        {
            AisleId = aislePrototype.AisleId,
            AisleName = aislePrototype.AisleName,
            Items = aislePrototype.Items
        };
}

public record CartEntryAisleResponsePrototype : CartEntryAisleResponse
{
    public required int SortOrder { get; init; }
    
    public static Expression<Func<IGrouping<Aisle?, CartEntry>, CartEntryAisleResponsePrototype>> FromAisleGroup =>
        aisleGroup => new CartEntryAisleResponsePrototype
        {
            AisleId = aisleGroup.Key != null ? aisleGroup.Key.AisleId : null,
            AisleName = aisleGroup.FirstOrDefault() != null ? aisleGroup.FirstOrDefault()!.Aisle!.AisleName : null,
            SortOrder = aisleGroup.Key != null ? aisleGroup.Key.SortOrder : -1,
            Items = aisleGroup
                .OrderBy(cir => cir.Bay)
                .ThenBy(cir => cir.Item.Temp)
                .ThenBy(cir => cir.ItemId)
                .ThenBy(cir => cir.Item.ItemName)
                .ThenBy(cir => cir.Prep != null ? cir.Prep.PrepName : null)
                .ThenBy(cir => cir.Prep != null ? cir.Prep.PrepId : Ulid.Empty)
                .Select(ce => new CartEntryItemResponse
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
                    Amounts = ce.Amounts,
                    IsChecked = ce.IsChecked,
                })
                .ToReadOnlyList()
        };
}