using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSyncBackend.Database.Objects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BayType
{
    Start,
    Middle,
    End
}

[UsedImplicitly]
public class BayTypeConverter() : ValueConverter<BayType, string>(
    v => v.ToString(),
    v => Enum.Parse<BayType>(v)
);