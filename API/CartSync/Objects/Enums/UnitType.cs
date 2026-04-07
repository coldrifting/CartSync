using System.Text.Json.Serialization;

namespace CartSync.Objects.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UnitType
{
    None = -1,
    
    Count = 1,
    
    VolumeTeaspoons = 8,
    VolumeTablespoons = 9,
    VolumeCups = 10,
    VolumeQuarts = 11,
    VolumePints = 12,
    VolumeGallons = 13,
    VolumeOunces = 14,
    
    WeightOunces = 32,
    WeightPounds = 33
}

public static class UnitTypeEx
{
    public static UnitTypeCategory GetCategory(UnitType unitType)
    {
        return unitType.IsVolume
            ? UnitTypeCategory.Volume
            : unitType.IsWeight
                ? UnitTypeCategory.Weight
                : UnitTypeCategory.Quantity;
    }
    
    extension(UnitType unitType)
    {
        public bool IsVolume => (int)unitType / 8 == 1;
        public bool IsWeight => (int)unitType / 8 == 4;

        public UnitTypeCategory Category => GetCategory(unitType);

        public bool IsCompatible(UnitType other)
        {
            int a = (int)unitType;
            int b = (int)other;

            return a / 8 == b / 8;
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

        public string Abbreviation =>
            unitType switch
            {
                UnitType.None => "(none)",
                UnitType.Count => "ea.",
                UnitType.VolumeTeaspoons => "tsp",
                UnitType.VolumeTablespoons => "Tbsp",
                UnitType.VolumeOunces => "oz",
                UnitType.VolumeCups => "cups",
                UnitType.VolumePints => "pt",
                UnitType.VolumeQuarts => "qt",
                UnitType.VolumeGallons => "gal",
                UnitType.WeightOunces => "oz (#)",
                UnitType.WeightPounds => "lbs",
                _ => throw new ArgumentOutOfRangeException(nameof(unitType), unitType, null)
            };

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
}