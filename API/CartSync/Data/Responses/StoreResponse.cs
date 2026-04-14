using System.Linq.Expressions;
using CartSync.Data.Entities;

namespace CartSync.Data.Responses;

public record StoreResponse
{
    public required Ulid Id { get; init; }
    public required string Name { get; init; }
    public required bool IsSelected { get; init; }
    
    public static Expression<Func<Store, StoreResponse>> FromEntity(Ulid selectedStoreId) =>
        store => new StoreResponse
        {
            Id = store.StoreId,
            Name = store.StoreName,
            IsSelected = store.StoreId == selectedStoreId
        };

    public static StoreResponse FromNewEntity(Store store) => FromEntity(Ulid.Empty).Compile()(store);
}