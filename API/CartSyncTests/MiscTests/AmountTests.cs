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
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify2()
    {
        Amount a = Amount.VolumeTeaspoons(5);
        Amount b = Amount.VolumeTeaspoons(2);
        
        Amount expected = Amount.VolumeTablespoons(7, 3);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify3()
    {
        Amount a = Amount.VolumeTeaspoons(5);
        Amount b = Amount.VolumeTeaspoons(4);
        
        Amount expected = Amount.VolumeTablespoons(3);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify4()
    {
        Amount a = Amount.VolumeTablespoons(15);
        Amount b = Amount.VolumeTablespoons(1);

        Amount expected = Amount.VolumeCups(1);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify5()
    {
        Amount a = Amount.VolumeTeaspoons(40);
        Amount b = Amount.VolumeTeaspoons(8);
        
        Amount expected = Amount.VolumeCups(1);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify6()
    {
        Amount a = Amount.VolumeTeaspoons(40);
        Amount b = Amount.VolumeTeaspoons(10);
        
        Amount expected = Amount.VolumeTablespoons(50, 3);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void TestAmountSimplify7()
    {
        Amount a = Amount.VolumeTeaspoons(40);
        Amount b = Amount.VolumeTeaspoons(11);
        
        Amount expected = Amount.VolumeTablespoons(17);
        Amount actual = a + b;
        
        Assert.Equal(expected, actual);
    }
}