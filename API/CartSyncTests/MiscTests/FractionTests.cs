using CartSync.Objects;

namespace CartSyncTests.MiscTests;

public class FractionTests
{
    [Theory]
    [InlineData("1.0", "1", 1, 1.0, false)]
    [InlineData("1.", "1", 1, 1.0, false)]
    [InlineData("1.00", "1", 1, 1.0, false)]
    [InlineData("001", "1", 1, 1.0, false)]
    [InlineData("001.000", "1", 1, 1.0, false)]
    [InlineData("001.1", "1.1", 1, 1.1, true)]
    [InlineData("0000", "0", 0, 0, false)]
    [InlineData("0010", "10", 10, 10, true)]
    [InlineData("0010.33", "10 ⅓", 10, 10.333, true)]
    [InlineData("005.333", "5 ⅓", 5, 5.333, true)]
    [InlineData("003.334", "3 ⅓", 3, 3.333, true)]
    [InlineData("1.66", "1 ⅔", 1, 1.666, true)]
    [InlineData("1.166", "1 ⅙", 1, 1.166, true)]
    [InlineData("1.834", "1 ⅚", 1, 1.833, true)]
    [InlineData("2.666", "2 ⅔", 2, 2.666, true)]
    [InlineData("3.667", "3 ⅔", 3, 3.666, true)]
    [InlineData("0.2", "⅕", 0, 0.2, false)]
    [InlineData("500.4", "500 ⅖", 500, 500.4, true)]
    [InlineData("500.6", "500 ⅗", 500, 500.6, true)]
    [InlineData("500.8", "500 ⅘", 500, 500.8, true)]
    [InlineData("20.25", "20 ¼", 20,20.25, true)]
    [InlineData("20.75", "20 ¾", 20,20.75, true)]
    [InlineData(".75", "¾", 0,0.75, false)]
    [InlineData(".166", "⅙", 0,0.166, false)]
    [InlineData("1.833", "1 ⅚", 1,1.833, true)]
    [InlineData("4.125", "4 ⅛", 4,4.125, true)]
    [InlineData("4.375", "4 ⅜", 4,4.375, true)]
    [InlineData("4.625", "4 ⅝", 4,4.625, true)]
    [InlineData("4.875", "4 ⅞", 4,4.875, true)]
    [InlineData("4.921", "4.92", 4,4.921, true)]
    public void TestFractionWhole(string input, string fracString, int expected, double expectedDouble, bool isPlural)
    {
        FixedPoint fp = new(input);
        Fraction frac = new(fp);
        Assert.Equal(expected, frac.AsInt);
        Assert.Equal(expectedDouble, frac.AsDouble, 0.01);
        Assert.Equal(fracString, frac.ToString());
        Assert.Equal(isPlural, frac.IsPlural);
    }
    
    [Theory]
    [InlineData(1, 1, 1, 1, 2, 1)]
    [InlineData(1, 2, 3, 4, 5, 4)]
    [InlineData(1, 3, 2, 3, 1, 1)]
    [InlineData(1, 3, 1, 1, 4, 3)]
    public void TestFractionAddMath(int num1, int dem1, int num2, int dem2, int numR, int demR)
    {
        Fraction frac1 = new(num1, dem1);
        Fraction frac2 = new(num2, dem2);

        Fraction expected = new(numR, demR);
        Fraction actual = frac1 + frac2;
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData(1, 1, 1, 1, 1, 1)]
    [InlineData(1, 2, 3, 4, 3, 8)]
    [InlineData(1, 3, 2, 3, 2, 9)]
    [InlineData(1, 3, 1, 1, 1, 3)]
    [InlineData(2, 3, 2, 4, 1, 3)]
    public void TestFractionMultiplyMath(int num1, int dem1, int num2, int dem2, int numR, int demR)
    {
        Fraction frac1 = new(num1, dem1);
        Fraction frac2 = new(num2, dem2);

        Fraction expected = new(numR, demR);
        Fraction actual = frac1 * frac2;
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData(2, 2, 1, 4, 1)]
    [InlineData(3, 1, 3, 1, 1)]
    [InlineData(5, 1, 10, 1, 2)]
    [InlineData(8, 1, 32, 1, 4)]
    public void TestFractionMultiplyScalarMath(int scalar, int num1, int dem2, int numR, int demR)
    {
        Fraction frac1 = new(num1, dem2);

        Fraction expected = new(numR, demR);
        Fraction actual = frac1 * scalar;
        
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData(2, 2, 1, 1, 1)]
    [InlineData(3, 1, 3, 1, 9)]
    [InlineData(5, 1, 10, 1, 50)]
    [InlineData(8, 1, 4, 1, 32)]
    public void TestFractionDivideScalarMath(int scalar, int num1, int dem2, int numR, int demR)
    {
        Fraction frac1 = new(num1, dem2);

        Fraction expected = new(numR, demR);
        Fraction actual = frac1 / scalar;
        
        Assert.Equal(expected, actual);
    }
}