using System.Text.Json.Serialization;

namespace CartSync.Objects.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Temp
{
    Ambient,
    Cold,
    Frozen
}