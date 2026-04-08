namespace CartSync.Data.Misc;

public record ItemPrepPair
{
    public required Ulid ItemId { get; init; }
    public required Ulid? PrepId { get; init; }

    public static Func<CartEntryPrototype, ItemPrepPair> FromCartEntryPrototype =>
        cartEntryPrototype => new ItemPrepPair
        {
            ItemId = cartEntryPrototype.ItemId, 
            PrepId = cartEntryPrototype.PrepId
        };
}