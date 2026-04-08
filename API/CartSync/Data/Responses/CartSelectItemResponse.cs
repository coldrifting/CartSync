using System.Linq.Expressions;
using CartSync.Data.Entities;
using CartSync.Objects;

namespace CartSync.Data.Responses;

public record CartSelectItemResponse
{
    public required ItemMinimalResponse Item { get; init; }
    public PrepResponse? Prep { get; init; }
    public Amount Amount { get; init; } = new();
    
    public static Expression<Func<CartSelectItem, CartSelectItemResponse>> FromEntity =>
        cartSelectItem => new CartSelectItemResponse
        {
            Item = new ItemMinimalResponse
            {
                Id = cartSelectItem.ItemId,
                Name = cartSelectItem.Item.ItemName,
                Temp = cartSelectItem.Item.Temp,
            },
            Prep = (cartSelectItem.PrepId == null
                ? null
                : new PrepResponse
                {
                    Id = cartSelectItem.PrepId.Value,
                    Name = cartSelectItem.Prep!.PrepName
                })!,
            Amount = cartSelectItem.Amounts.Amount
        };
}