using CartSync.Objects;

namespace CartSyncTests.MiscTests;

public class AmountTests
{
    [Fact]
    public void TestAmountSimplify()
    {
        Amount a = Amount.VolumeTeaspoons(1);
        Amount b = Amount.VolumeTeaspoons(2);

        Amount expected = Amount.VolumeTablespoons(1);
        Amount actual = a.Add(b);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify2()
    {
        Amount a = Amount.VolumeTeaspoons(5);
        Amount b = Amount.VolumeTeaspoons(2);
        
        Amount expected = Amount.VolumeTablespoons(7, 3);
        Amount actual = a.Add(b);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify3()
    {
        Amount a = Amount.VolumeTeaspoons(5);
        Amount b = Amount.VolumeTeaspoons(4);
        
        Amount expected = Amount.VolumeTablespoons(3);
        Amount actual = a.Add(b);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify4()
    {
        Amount a = Amount.VolumeTablespoons(15);
        Amount b = Amount.VolumeTablespoons(1);

        Amount expected = Amount.VolumeCups(1);
        Amount actual = a.Add(b);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify5()
    {
        Amount a = Amount.VolumeTeaspoons(40);
        Amount b = Amount.VolumeTeaspoons(8);
        
        Amount expected = Amount.VolumeCups(1);
        Amount actual = a.Add(b);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify6()
    {
        Amount a = Amount.VolumeTeaspoons(40);
        Amount b = Amount.VolumeTeaspoons(10);
        
        Amount expected = Amount.VolumeTablespoons(50, 3);
        Amount actual = a.Add(b);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify7()
    {
        Amount a = Amount.VolumeTeaspoons(40);
        Amount b = Amount.VolumeTeaspoons(11);
        
        Amount expected = Amount.VolumeTablespoons(17);
        Amount actual = a.Add(b);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify8()
    {
        Amount a = Amount.WeightOunces(8);
        
        Amount expected = Amount.WeightPounds(1);
        Amount actual = a.Multiply(2);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify9()
    {
        Amount a = Amount.VolumeCups(4);
        
        Amount expected = Amount.VolumeQuarts(1);
        Amount actualCapped = a.Add(Amount.None);
        Amount actualUncapped = a.Add(Amount.None, true);
        
        Assert.Equal(a, actualCapped);
        Assert.Equal(expected, actualUncapped);
    }
    
    [Fact]
    public void TestAmountSimplify10()
    {
        Amount a = Amount.VolumeCups(4);
        
        Amount expected = Amount.VolumeQuarts(1);
        Amount actual = a.Multiply(1, true);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify11()
    {
        Amount a = Amount.VolumeCups(2);
        
        Amount expected = Amount.VolumePints(1);
        Amount actual = a.Multiply(1, true);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify12()
    {
        Amount a = Amount.VolumeCups(2);
        Amount b = Amount.VolumeOunces(8);

        Amount expected = Amount.VolumeCups(3);
        Amount actual = a.Add(b);
        Amount actual2 = b.Add(a);
        
        Assert.Equal(expected, actual);
        Assert.Equal(expected, actual2);
    }
    
    [Fact]
    public void TestAmountSimplify13()
    {
        Amount a = Amount.VolumeOunces(8);
        Amount b = Amount.VolumeOunces(8);

        Amount expected = Amount.VolumeOunces(16);
        Amount actual = a.Add(b);
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify14()
    {
        Amount a = Amount.VolumeOunces(8);

        Amount expected = Amount.VolumeOunces(16);
        Amount actual = a.Multiply(2);
        
        Assert.Equal(expected, actual);
    }
}