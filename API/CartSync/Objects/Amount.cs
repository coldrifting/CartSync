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
    
    public Amount Simplify()
    {
        Amount amt = this;
        
        if (amt is { UnitType: UnitType.VolumeTeaspoons, Fraction.AsInt: >= 3 })
        {
            amt = new Amount(UnitType.VolumeTablespoons, amt.Fraction / 3);
        }
        
        if (amt is { UnitType: UnitType.VolumeTablespoons, Fraction.AsInt: >= 16 })
        {
            // Avoid "Simplifying" to weird fractions
            Fraction newFrac = amt.Fraction / 16;
            if (newFrac.Dem > 10 && newFrac.Dem > amt.Fraction.Dem)
            {
                return amt;
            }
            
            return new Amount(UnitType.VolumeCups, newFrac);
        }

        return amt;
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
        
        return left.UnitType.IsCompatible(right.UnitType) 
            ? new Amount(left.UnitType, left.Fraction + right.Fraction).Simplify() 
            : new Amount(UnitType.None, 0);
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