using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSyncBackend.Database.Objects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ItemTemp
{
    Ambient,
    Cold,
    Frozen
}
[UsedImplicitly]
public class ItemTempConverter() : ValueConverter<ItemTemp, string>(
    v => v.ToString(),
    v => Enum.Parse<ItemTemp>(v)
);