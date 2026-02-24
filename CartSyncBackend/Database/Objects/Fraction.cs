using System.Globalization;

namespace CartSyncBackend.Database.Objects;

public class Fraction
{
    public int Num { get; set; }
    public int Dem { get; set; } = 1;

    public int AsInt => Num / Dem;
    public double AsDouble => Num / (double)Dem;
    
    public string DecimalString => ((int)(Num * 1000.0) / (double)(Dem) / 1000.0).ToString(CultureInfo.InvariantCulture);
    public bool IsPlural => (Num / (float)Dem) > 1.0 || this.ToString().Contains('.');

    public Fraction(FixedPoint input)
    {
        int whole = input.AsInt;
        int frac = input.Fraction;

        if (frac == 0)
        {
            Num = whole;
            return;
        }

        switch (frac)
        {
            case 160:
            case 166:
            case 167:
                Num = (whole * 6) + 1;
                Dem = 6;
                return;
            
            case 830:
            case 833:
            case 834:
                Num = (whole * 6) + 5;
                Dem = 6;
                return;
            
            case 330:
            case 333:
            case 334:
                Num = (whole * 3) + 1;
                Dem = 3;
                return;
                
            case 660:
            case 666:
            case 667:
                Num = (whole * 3) + 2;
                Dem = 3;
                return;
        }

        for (int divisor = 2; divisor <= 16; divisor++)
        {
            if (((frac * divisor) % 1000) == 0)
            {
                Num = (whole * divisor) + ((frac * divisor) / 1000);
                Dem = divisor;
                return;
            }
        }
        
        // Fallback
        Num = (whole * 1000) + frac;
        Dem = 1000;
    }

    public Fraction(string input)
    {
        Fraction frac = new(new FixedPoint(input));
        Num = frac.Num;
        Dem = frac.Dem;
    }
    
    public Fraction(int num, int dem)
    {
        Num = num;
        Dem = dem;
    }

    public Fraction(int num)
    {
        Num = num;
        Dem = 1;
    }

    public override string ToString()
    {
        if (Num > Dem)
        {
            int whole = Num / Dem;
            int partial = Num % Dem;

            if (partial == 0)
            {
                return whole.ToString();
            }

            string output = whole + " " + new Fraction(partial, Dem);

            return output.Replace("0.", ".");
        }

        switch ($"{Num}/{Dem}")
        {
            case "1/2": return "½";

            case "1/3":
            case "33/100":
            case "333/1000": return "⅓";
            
            case "2/3":
            case "66/100":
            case "666/1000": return "⅔";
            
            case "1/4": return "¼";
            case "3/4": return "¾";

            case "1/5": return "⅕";
            case "2/5": return "⅖";
            case "3/5": return "⅗";
            case "4/5": return "⅘";

            case "1/6": return "⅙";
            case "5/6": return "⅚";

            case "1/8": return "⅛";
            case "3/8": return "⅜";
            case "5/8": return "⅝";
            case "7/8": return "⅞";
            
            default:
                string test = (Num / (float)Dem).ToString("%g");
                return test[..4];
        }
    }

    public static Fraction operator +(Fraction left, Fraction right)
    {
        if (left.Dem == right.Dem)
        {
            return new Fraction(left.Num + right.Num, left.Dem).Simplify();
        }
        
        int lcd = left.Dem * right.Dem;
        int leftMult = lcd / left.Dem;
        int rightMult = lcd / right.Dem;

        return new Fraction(left.Num * leftMult + right.Num * rightMult, lcd);
    }

    public static Fraction operator *(Fraction left, Fraction right)
    {
        return new Fraction(left.Num * right.Num, left.Dem * right.Dem).Simplify();
    }

    public static Fraction operator *(Fraction left, int right)
    {
        return new Fraction(left.Num * right, left.Dem).Simplify();
    }

    public static Fraction operator /(Fraction left, int right)
    {
        return new Fraction(left.Num, left.Dem * right).Simplify();
    }
    
    public Fraction Simplify()
    {
        for (int div = Dem; div >= 2; div--)
        {
            if (Num % div == 0 && Dem % div == 0)
            {
                return new Fraction(Num / div, Dem / div);
            }
        }

        return this;
    }
}