using CartSync.Objects.Enums;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSync.Objects;

public class AmountGroup
{
    public static AmountGroup None => new(Amount.None);

    public Amount Count { get; init; } = Amount.None;
    public Amount Volume { get; init; } = Amount.None;
    public Amount Weight { get; init; } = Amount.None;

    public AmountGroup()
    {
    }
    
    public AmountGroup(Amount amount)
    {
        if (UnitTypeEx.GetCategory(amount.UnitType) == UnitTypeCategory.Quantity)
        {
            Count = amount;
        }
        else if (UnitTypeEx.GetCategory(amount.UnitType) == UnitTypeCategory.Volume)
        {
            Volume = amount;
        }
        else if (UnitTypeEx.GetCategory(amount.UnitType) == UnitTypeCategory.Weight)
        {
            Weight = amount;
        }
    }

    public AmountGroup Add(AmountGroup other, bool uncapUnits = false)
    {
        return new AmountGroup
        {
            Count =  IsNonEmpty(Count, other.Count) ? Count.Add(other.Count, uncapUnits) : Amount.None,
            Volume = IsNonEmpty(Volume, other.Volume) ? Volume.Add(other.Volume, uncapUnits) : Amount.None,
            Weight = IsNonEmpty(Weight, other.Weight) ? Weight.Add(other.Weight, uncapUnits) : Amount.None,
        };
    }

    private static bool IsNonEmpty(Amount left, Amount right)
    {
        return !left.Equals(Amount.None) || !right.Equals(Amount.None);
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is AmountGroup other)
        {
            return Equals(Count, other.Count) && 
                   Equals(Volume, other.Volume) &&
                   Equals(Weight, other.Weight);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Count.GetHashCode() ^ Volume.GetHashCode() ^ Weight.GetHashCode();
    }
    
    public Amount Amount =>
        Volume.Fraction.Num > 0 
            ? Volume 
            : Weight.Fraction.Num > 0 
                ? Weight 
                : Count;

    private string DbString => $"Count: {Count.DbString} | Volume: {Volume.DbString} | Weight: {Weight.DbString}";

    private static AmountGroup FromDbString(string s)
    {
        string[] sections = s.Split('|', 3);

        string countString = sections[0].Split(":")[1].Trim();
        string volumeString = sections[1].Split(":")[1].Trim();
        string weightString = sections[2].Split(":")[1].Trim();
        
        return new AmountGroup
        {
            Count = Amount.FromDbString(countString),
            Volume = Amount.FromDbString(volumeString),
            Weight = Amount.FromDbString(weightString)
        };
    }
    
    [UsedImplicitly]
    public class ValueConverter() : ValueConverter<AmountGroup, string>(
        v => v.DbString,
        v => FromDbString(v)
    );
}