using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CartSyncBackend.Database.Objects;

public class Amount
{
    public Fraction Fraction { get; set; }
    public UnitType UnitType { get; set; }

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
        
        if (amt is { UnitType: UnitType.VolumeTeaspoons, Fraction.AsInt: >= 16 })
        {
            return new Amount(amt.Fraction / 16, UnitType.VolumeCups);
        }

        return amt;
    }
    
    public override string ToString()
    {
        return Fraction + " " + UnitType.GetAbbreviation(Fraction.IsPlural);
    }
}

[UsedImplicitly]
public class AmountConverter() : ValueConverter<Amount, string>(
    v => v.Fraction + "," + v.UnitType,
    v => new Amount()
    );