using System.Text.RegularExpressions;

namespace CartSync.Objects;

public partial class FixedPoint
{
    private int Backing { get; set; }

    public int AsInt => Backing / 1000;
    public double AsDouble => Backing / 1000.0;
    
    /// The fractional part, expressed out of 1000
    public int Fraction => Backing % 1000;
    
    public FixedPoint(int num)
    {
        Backing = num * 1000;
    }

    public FixedPoint(double num)
    {
        Backing = (int)(num * 1000);
    }

    public FixedPoint(string num)
    {
        if (!NumberWithOptionalDecimalRegex().IsMatch(num))
        {
            Backing = 0;
            return;
        }

        Backing = (int)((double.TryParse(num, out double fraction) ? fraction : 0) * 1000.0);
    }

    public override string ToString()
    {
        string asString = Backing.ToString();

        if (Backing < 1000)
        {
            asString = asString.Insert(0, "0");
        }
        if (Backing < 100)
        {
            asString = asString.Insert(0, "0");
        }
        if (Backing < 10)
        {
            asString = asString.Insert(0, "0");
        }
        
        return asString.Insert(asString.Length - 3, ".");
    }

    [GeneratedRegex("""^[\d,]*\.?[\d,]*$""")]
    private static partial Regex NumberWithOptionalDecimalRegex();
}