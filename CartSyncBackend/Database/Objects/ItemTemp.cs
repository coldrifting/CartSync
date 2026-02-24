using System.Text.Json.Serialization;

namespace CartSyncBackend.Database.Objects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ItemTemp
{
    Ambient,
    Cold,
    Frozen
}