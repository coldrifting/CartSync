using CartSync.Objects;

namespace CartSyncTests.MiscTests;

public class FixedPointTests
{
    [Theory]
    [InlineData("abc")]
    [InlineData("12a")]
    [InlineData("f37")]
    [InlineData("0-0")]
    [InlineData("..0")]
    [InlineData("5..0")]
    public void TestFromInvalidString(string input)
    {
        FixedPoint fp = new(input);
        Assert.Equal(0, fp.AsInt);
    }

    [Theory]
    [InlineData("0000", 0, 0.0, 0, "0.000")]
    [InlineData("000", 0, 0.0, 0, "0.000")]
    [InlineData("00", 0, 0.0, 0, "0.000")]
    [InlineData("0", 0, 0.0, 0, "0.000")]
    [InlineData("0.0", 0, 0.0, 0, "0.000")]
    [InlineData("0.000", 0, 0.0, 0, "0.000")]
    [InlineData("0.0000", 0, 0.0, 0, "0.000")]
    [InlineData("0.00000", 0, 0.0, 0, "0.000")]
    [InlineData("1", 1, 1.0, 0, "1.000")]
    [InlineData("1.0", 1, 1.0, 0, "1.000")]
    [InlineData("1.000", 1, 1.0, 0, "1.000")]
    [InlineData("1.0000", 1, 1.0, 0, "1.000")]
    [InlineData("1.3333", 1, 1.333, 333, "1.333")]
    [InlineData("1.6666", 1, 1.666, 666, "1.666")]
    [InlineData("2.0", 2, 2.0, 0, "2.000")]
    [InlineData("0.5", 0, 0.5, 500, "0.500")]
    [InlineData("54", 54, 54.0, 0, "54.000")]
    public void TestFromValidString(string input, int expected, double expectedDouble, int expectedFraction, string expectedString)
    {
        FixedPoint fp = new(input);
        Assert.Equal(expected, fp.AsInt);
        Assert.Equal(expectedDouble, fp.AsDouble);
        Assert.Equal(expectedFraction, fp.Fraction);
        Assert.Equal(expectedString, fp.ToString());
    }

    [Theory]
    [InlineData(0, 0, 0.0, 0, "0.000")]
    [InlineData(1, 1, 1.0, 0, "1.000")]
    [InlineData(2, 2, 2.0, 0, "2.000")]
    [InlineData(54, 54, 54.0, 0, "54.000")]
    public void TestFromInt(int input, int expected, double expectedDouble, int expectedFraction, string expectedString)
    {
        FixedPoint fp = new(input);
        Assert.Equal(expected, fp.AsInt);
        Assert.Equal(expectedDouble, fp.AsDouble);
        Assert.Equal(expectedFraction, fp.Fraction);
        Assert.Equal(expectedString, fp.ToString());
    }

    [Theory]
    [InlineData(0, 0, 0.0, 0, "0.000")]
    [InlineData(1, 1, 1.0, 0, "1.000")]
    [InlineData(1.0, 1, 1.0, 0, "1.000")]
    [InlineData(1.3333, 1, 1.333, 333, "1.333")]
    [InlineData(1.6666, 1, 1.666, 666, "1.666")]
    [InlineData(2.0, 2, 2.0, 0, "2.000")]
    [InlineData(0.5, 0, 0.5, 500, "0.500")]
    [InlineData(54, 54, 54.0, 0, "54.000")]
    public void TestFromDouble(double input, int expected, double expectedDouble, int expectedFraction, string expectedString)
    {
        FixedPoint fp = new(input);
        Assert.Equal(expected, fp.AsInt);
        Assert.Equal(expectedDouble, fp.AsDouble);
        Assert.Equal(expectedFraction, fp.Fraction);
        Assert.Equal(expectedString, fp.ToString());
    }
}