using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSyncBackend.Objects.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BayType
{
    Start,
    Middle,
    End
}

public static class BayTypeEx
{
    [UsedImplicitly]
    public class ValueConverter() : ValueConverter<BayType, string>(
        v => v.ToString(),
        v => Enum.Parse<BayType>(v)
    );
}