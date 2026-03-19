using CartSync.Objects.Enums;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSync.Objects;

public class Amount
{
    public Fraction Fraction { get; }
    public UnitType UnitType { get; }

    public static readonly Amount None = new(0, UnitType.None);
    
    public Amount()
    {
        Fraction = new Fraction(1);
        UnitType = UnitType.Count;
    }

    public Amount(Fraction fraction, UnitType unitType)
    {
        Fraction = fraction;
        UnitType = unitType;
    }

    public Amount(int quantity, UnitType unitType)
    {
        Fraction = new Fraction(quantity);
        UnitType = unitType;
    }
    
    public Amount Simplify()
    {
        Amount amt = this;
        
        if (amt is { UnitType: UnitType.VolumeTeaspoons, Fraction.AsInt: >= 3 })
        {
            amt = new Amount(amt.Fraction / 3, UnitType.VolumeTablespoons);
        }
        
        if (amt is { UnitType: UnitType.VolumeTablespoons, Fraction.AsInt: >= 16 })
        {
            // Avoid "Simplifying" to weird fractions
            Fraction newFrac = amt.Fraction / 16;
            if (newFrac.Dem > 10 && newFrac.Dem > amt.Fraction.Dem)
            {
                return amt;
            }
            
            return new Amount(newFrac, UnitType.VolumeCups);
        }

        return amt;
    }

    public static Amount operator +(Amount left, Amount right)
    {
        return left.UnitType.IsCompatible(right.UnitType) 
            ? new Amount(left.Fraction + right.Fraction, left.UnitType).Simplify() 
            : new Amount(-1, UnitType.None);
    }

    public static Amount operator *(Amount left, int right)
    {
        return new Amount(left.Fraction * right, left.UnitType).Simplify();
    }

    public static Amount operator *(int left, Amount right)
    {
        return new Amount(right.Fraction * left, right.UnitType).Simplify();
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
    

    [UsedImplicitly]
    public class ValueConverter() : ValueConverter<Amount, string>(
        v => v.Fraction + "," + v.UnitType,
        v => new Amount()
    );
}