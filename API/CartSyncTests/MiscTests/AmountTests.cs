using CartSync.Objects;
using CartSync.Objects.Enums;

namespace CartSyncTests.MiscTests;

public class AmountTests
{
    [Fact]
    public void TestAmountSimplify()
    {
        Amount a = new(1, UnitType.VolumeTeaspoons);
        Amount b = new(2, UnitType.VolumeTeaspoons);
        
        Amount expected = new(1, UnitType.VolumeTablespoons);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify2()
    {
        Amount a = new(5, UnitType.VolumeTeaspoons);
        Amount b = new(2, UnitType.VolumeTeaspoons);
        
        Amount expected = new(new Fraction(7, 3), UnitType.VolumeTablespoons);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify3()
    {
        Amount a = new(5, UnitType.VolumeTeaspoons);
        Amount b = new(4, UnitType.VolumeTeaspoons);
        
        Amount expected = new(new Fraction(3, 1), UnitType.VolumeTablespoons);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify4()
    {
        Amount a = new(15, UnitType.VolumeTablespoons);
        Amount b = new(1, UnitType.VolumeTablespoons);
        
        Amount expected = new(new Fraction(1, 1), UnitType.VolumeCups);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify5()
    {
        Amount a = new(40, UnitType.VolumeTeaspoons);
        Amount b = new(8, UnitType.VolumeTeaspoons);
        
        Amount expected = new(new Fraction(1, 1), UnitType.VolumeCups);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify6()
    {
        Amount a = new(40, UnitType.VolumeTeaspoons);
        Amount b = new(10, UnitType.VolumeTeaspoons);
        
        Amount expected = new(new Fraction(50, 3), UnitType.VolumeTablespoons);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify7()
    {
        Amount a = new(40, UnitType.VolumeTeaspoons);
        Amount b = new(11, UnitType.VolumeTeaspoons);
        
        Amount expected = new(new Fraction(17, 1), UnitType.VolumeTablespoons);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
}