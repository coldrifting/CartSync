using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSyncBackend.Database.Objects;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UnitType
{
    None,
    Count,
    VolumeTeaspoons,
    VolumeTablespoons,
    VolumeOunces,
    VolumeCups,
    VolumeQuarts,
    VolumePints,
    VolumeGallons,
    WeightOunces,
    WeightPounds
}

public static class UnitTypeExtensions
{
    public static string GetAbbreviation(this UnitType unitType, bool isPlural = false)
    {
        return unitType switch
        {
            UnitType.None => "(none)",
            UnitType.Count => "ea.",
            UnitType.VolumeTeaspoons => "tsp",
            UnitType.VolumeTablespoons => "Tbsp",
            UnitType.VolumeOunces => "oz",
            UnitType.VolumeCups => isPlural ? "cups" : "cup",
            UnitType.VolumePints => "pt",
            UnitType.VolumeQuarts => "qt",
            UnitType.VolumeGallons => "gal",
            UnitType.WeightOunces => "oz",
            UnitType.WeightPounds => isPlural ? "lbs" : "lb",
            _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
        };
    }

    public static int GetUnits(this UnitType unitType)
    {
        return unitType switch
        {
            UnitType.None => 0,
            UnitType.Count => 1,
            UnitType.VolumeTeaspoons => 1,
            UnitType.VolumeTablespoons => 3,
            UnitType.VolumeOunces => 6,
            UnitType.VolumeCups => 48,
            UnitType.VolumePints => 96,
            UnitType.VolumeQuarts => 192,
            UnitType.VolumeGallons => 768,
            UnitType.WeightOunces => 1,
            UnitType.WeightPounds => 16,
            _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
        };
    }
}

[UsedImplicitly]
public class UnitTypeConverter() : ValueConverter<UnitType, string>(
    v => v.ToString(),
    v => Enum.Parse<UnitType>(v)
);