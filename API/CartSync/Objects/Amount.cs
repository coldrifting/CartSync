using System.Diagnostics.CodeAnalysis;
using CartSync.Objects.Enums;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSync.Objects;

// Needed for deserialization
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class Amount
{
    public UnitType UnitType { get; init; }
    public Fraction Fraction { get; init; }

    public static readonly Amount None = new(UnitType.None, 0);
    
    public Amount()
    {
        UnitType = UnitType.Count;
        Fraction = new Fraction(1);
    }

    private Amount(UnitType unitType, Fraction fraction)
    {
        UnitType = unitType;
        Fraction = fraction;
    }

    private Amount(UnitType unitType, int num, int dem = 1)
    {
        UnitType = unitType;
        Fraction = new Fraction(num, dem);
    }

    public Amount Combine(Amount other)
    {
        if (UnitType is { IsVolume: false, IsWeight: false })
        {
            return new Amount(UnitType, Fraction + other.Fraction).Simplify();
        }

        Fraction a = Fraction * UnitType.Units;
        Fraction b = other.Fraction * UnitType.Units;
        UnitType baseUnitType = UnitType.IsVolume ? UnitType.VolumeTeaspoons : UnitType.WeightOunces;
        return new Amount(baseUnitType, a + b).Simplify();
    }
    
    public Amount Simplify()
    {
        return Simplify(this);
    }

    private static Amount Simplify(Amount amount)
    {
        switch (amount.UnitType)
        {
            case UnitType.WeightOunces:
            {
                return StepUp(amount, 16, UnitType.WeightPounds);
            }
            case UnitType.VolumeTeaspoons:
            {
                return StepUp(amount, 3, UnitType.VolumeTablespoons);
            }
            case UnitType.VolumeTablespoons:
            {
                return StepUp(amount, 16, UnitType.VolumeCups);
            }
            case UnitType.VolumeOunces:
            {
                return StepUp(amount, 8, UnitType.VolumeCups);
            }
            case UnitType.VolumeCups:
            {
                return StepUp(amount, 2, UnitType.VolumePints);
            }
            case UnitType.VolumeQuarts:
            {
                return StepUp(amount, 2, UnitType.VolumeQuarts);
            }
            case UnitType.VolumePints:
            {
                return StepUp(amount, 2, UnitType.VolumeGallons);
            }
            case UnitType.None:
            case UnitType.Count:
            case UnitType.WeightPounds:
            case UnitType.VolumeGallons:
            default:
                return amount;
        }
    }

    private static Amount StepUp(Amount amount, int threshold, UnitType nextUnitType)
    {
        const int demThreshold = 5;
        Fraction newFrac = amount.Fraction / threshold;
        return amount.Fraction.AsInt >= threshold && newFrac.Dem <= demThreshold 
            ? new Amount(nextUnitType, newFrac).Simplify()
            : amount;
    }

    public static Amount Count(int num, int dem = 1) => new(UnitType.Count, num, dem);
    public static Amount VolumeTeaspoons(int num, int dem = 1) => new(UnitType.VolumeTeaspoons, num, dem);
    public static Amount VolumeTablespoons(int num, int dem = 1) => new(UnitType.VolumeTablespoons, num, dem);
    public static Amount VolumeOunces(int num, int dem = 1) => new(UnitType.VolumeOunces, num, dem);
    public static Amount VolumeCups(int num, int dem = 1) => new(UnitType.VolumeCups, num, dem);
    public static Amount VolumeQuarts(int num, int dem = 1) => new(UnitType.VolumeQuarts, num, dem);
    public static Amount VolumePints(int num, int dem = 1) => new(UnitType.VolumePints, num, dem);
    public static Amount VolumeGallons(int num, int dem = 1) => new(UnitType.VolumeGallons, num, dem);
    public static Amount WeightOunces(int num, int dem = 1) => new(UnitType.WeightOunces, num, dem);
    public static Amount WeightPounds(int num, int dem = 1) => new(UnitType.WeightPounds, num, dem);
    
    public static Amount operator +(Amount left, Amount right)
    {
        if (left.UnitType == UnitType.None)
        {
            return right;
        }

        if (right.UnitType == UnitType.None)
        {
            return left;
        }

        return !left.UnitType.IsCompatible(right.UnitType) 
            ? throw new InvalidOperationException($"Unable to add amounts with incompatible unit types: {left} + {right}") 
            : left.Combine(right);
    }

    public static Amount operator *(Amount left, int right)
    {
        return left.UnitType == UnitType.None 
            ? None 
            : new Amount(left.UnitType, left.Fraction * right).Simplify();
    }

    public static Amount operator *(int left, Amount right)
    {
        return right.UnitType == UnitType.None 
            ? None 
            : new Amount(right.UnitType, right.Fraction * left).Simplify();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Amount other)
        {
            return Equals(Fraction, other.Fraction) && Equals(UnitType, other.UnitType);
        }

        return false;
    }

    public override int GetHashCode()
    {
        return Fraction.GetHashCode() ^ UnitType.GetHashCode();
    }

    public override string ToString()
    {
        return Fraction + " " + UnitType.GetAbbreviation(Fraction.IsPlural);
    }
    
    private string ToDbString()
    {
        return Fraction.Num + "/" + Fraction.Dem + "," + UnitType;
    }

    private static Amount FromDbString(string s)
    {
        string[] first = s.Split('/', 2);
        int num = int.Parse(first[0]);

        string[] second = first[1].Split(',');
        int dem = int.Parse(second[0]);

        UnitType unit = Enum.Parse<UnitType>(second[1]);
        
        return new Amount(unit, new Fraction(num, dem));
    }
    

    [UsedImplicitly]
    public class ValueConverter() : ValueConverter<Amount, string>(
        v => v.ToDbString(),
        v => FromDbString(v)
    );
}