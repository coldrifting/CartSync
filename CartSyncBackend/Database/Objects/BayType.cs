using System.Text.Json.Serialization;

namespace CartSyncBackend.Database.Objects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum BayType
{
    Start,
    Middle,
    End
}