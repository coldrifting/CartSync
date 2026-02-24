using System.Text.RegularExpressions;

namespace CartSyncBackend.Database.Objects;

public class FixedPoint
{
    private int Backing { get; set; }

    public int AsInt => Backing / 1000;
    public double AsDouble => Backing / 1000.0;
    
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
        if (!Regex.IsMatch(num, @"^[a-z\d,]*\.?[a-z\d,]*$"))
        {
            Backing = 0;
            return;
        }

        string curString = "0" + num;
        if (!curString.Contains('.'))
        {
            curString = curString + ".0000";
        }
        else
        {
            curString = curString + "0000";
        }
        
        int index = curString.IndexOf('.');
        int index2 = index + 3;

        curString = curString.Remove(index);
        curString = curString.Insert(index2, ".");

        string finalString = curString[..index2];

        Backing = int.TryParse(finalString, out int fraction) ? fraction : 0;
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
}