using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CartSync.Objects.Enums;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSync.Objects;

// Needed for deserialization
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[DebuggerDisplay("{DebugString}")]
public class Amount
{
    public UnitType UnitType { get; init; }
    public Fraction Fraction { get; init; }
    
    private string DebugString => $"{Fraction.Num}/{Fraction.Dem} - {UnitType.Abbreviation}";

    public static readonly Amount None = new(UnitType.None, new Fraction(0));
    
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
    
    public Amount Add(Amount other, bool uncapUnits = false)
    {
        if (UnitType == UnitType.None)
        {
            return other.Simplify(uncapUnits);
        }

        if (other.UnitType == UnitType.None)
        {
            return Simplify(uncapUnits);
        }

        if (!other.UnitType.IsCompatible(UnitType))
        {
            throw new InvalidOperationException(
                $"Unable to add amounts with incompatible unit types: {DebugString} + {other.DebugString}");
        }

        switch (UnitType)
        {
            // Ounces stay ounces unless combined with other unit types (useful for cans)
            case UnitType.VolumeOunces when other.UnitType == UnitType.VolumeOunces:
                return new Amount(UnitType, Fraction + other.Fraction);
            case { IsVolume: false, IsWeight: false }:
                return new Amount(UnitType, Fraction + other.Fraction).Simplify(uncapUnits);
        }

        Fraction a = Fraction * UnitType.Units;
        Fraction b = other.Fraction * other.UnitType.Units;
        UnitType baseUnitType = UnitType.IsVolume ? UnitType.VolumeTeaspoons : UnitType.WeightOunces;
        return new Amount(baseUnitType, a + b).Simplify(uncapUnits);
    }
    
    public Amount Multiply(int factor, bool uncapUnits = false)
    {
        if (UnitType == UnitType.None || factor == 0)
        {
            return None;
        }

        return WithFraction(Fraction * factor).Simplify(uncapUnits);
    }

    private Amount Simplify(bool uncapUnits)
    {
        return UnitType switch
        {
            UnitType.WeightOunces => StepUp(16, UnitType.WeightPounds, uncapUnits),
            UnitType.VolumeTeaspoons => StepUpVolume(3, UnitType.VolumeTablespoons, uncapUnits),
            UnitType.VolumeTablespoons => StepUpVolume(16, UnitType.VolumeCups, uncapUnits),
            UnitType.VolumeCups => StepUpVolume(2, UnitType.VolumePints, uncapUnits),
            UnitType.VolumePints => StepUpVolume(2, UnitType.VolumeQuarts, uncapUnits),
            UnitType.VolumeQuarts => StepUpVolume(4, UnitType.VolumeGallons, uncapUnits),
            _ => this
        };
    }
    
    private Amount StepUpVolume(int threshold, UnitType nextUnitType, bool uncapUnits)
    {
        if (nextUnitType > UnitType.VolumeCups && !uncapUnits)
        {
            return this;
        }
        
        return StepUp(threshold, nextUnitType, uncapUnits);
    }
    
    private Amount StepUp(int threshold, UnitType nextUnitType, bool uncapUnits)
    {
        const int demThreshold = 5;
        Fraction newFrac = Fraction / threshold;
        return Fraction.AsInt >= threshold && newFrac.Dem <= demThreshold 
            ? new Amount(nextUnitType, newFrac).Simplify(uncapUnits)
            : this;
    }

    public static Amount Count(int num, int dem = 1) => new(UnitType.Count, new Fraction(num, dem));
    
    public static Amount VolumeTeaspoons(int num, int dem = 1) => new(UnitType.VolumeTeaspoons, new Fraction(num, dem));
    public static Amount VolumeTablespoons(int num, int dem = 1) => new(UnitType.VolumeTablespoons, new Fraction(num, dem));
    public static Amount VolumeOunces(int num, int dem = 1) => new(UnitType.VolumeOunces, new Fraction(num, dem));
    public static Amount VolumeCups(int num, int dem = 1) => new(UnitType.VolumeCups, new Fraction(num, dem));
    public static Amount VolumePints(int num, int dem = 1) => new(UnitType.VolumePints, new Fraction(num, dem));
    public static Amount VolumeQuarts(int num, int dem = 1) => new(UnitType.VolumeQuarts, new Fraction(num, dem));
    public static Amount VolumeGallons(int num, int dem = 1) => new(UnitType.VolumeGallons, new Fraction(num, dem));
    
    public static Amount WeightOunces(int num, int dem = 1) => new(UnitType.WeightOunces, new Fraction(num, dem));
    public static Amount WeightPounds(int num, int dem = 1) => new(UnitType.WeightPounds, new Fraction(num, dem));
    
    public Amount WithFraction(Fraction fraction)
    {
        return new Amount(UnitType, fraction);
    }
    
    public static implicit operator AmountGroup(Amount amount) => new(amount);
    
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
    
    public string DbString => Fraction.Num + "/" + Fraction.Dem + "," + UnitType;

    public static Amount FromDbString(string s)
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
        v => v.DbString,
        v => FromDbString(v)
    );
}