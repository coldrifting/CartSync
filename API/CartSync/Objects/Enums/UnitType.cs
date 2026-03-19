using System.Text.Json.Serialization;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSync.Objects.Enums;

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

public static class UnitTypeEx
{
    extension(UnitType unitType)
    {
        public bool IsCompatible(UnitType other)
        {
            if (unitType.ToString().StartsWith("Volume") && other.ToString().StartsWith("Volume"))
            {
                return true;
            }
            if (unitType.ToString().StartsWith("Weight") && other.ToString().StartsWith("Weight"))
            {
                return true;
            }

            return unitType == other;
        }
        
        public string GetAbbreviation(bool isPlural = false)
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

        public int Units =>
            unitType switch
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

    [UsedImplicitly]
    public class ValueConverter() : ValueConverter<UnitType, string>(
        v => v.ToString(),
        v => Enum.Parse<UnitType>(v)
    );
}