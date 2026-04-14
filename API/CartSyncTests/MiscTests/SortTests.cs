using CartSync.Interfaces;
using CartSync.Utils;

namespace CartSyncTests.MiscTests;

public class SortTests
{
    private class SortingExample(int id, string value) : ISortable
    {
        public int SortOrder { get; set; } = id;
        public string Value { get; } = value;

        public override bool Equals(object? obj)
        {
            if (obj is not SortingExample test)
            {
                return false;
            }
            
            return test.SortOrder == SortOrder && test.Value == Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
    
    private class SortingExampleListComparer : IEqualityComparer<IEnumerable<SortingExample>>
    {
        public bool Equals(IEnumerable<SortingExample>? x, IEnumerable<SortingExample>? y)
        {
            if (x == null || y == null)
            {
                return x == null && y == null;
            }
            
            SortingExample[] xArr = x.ToArray();
            SortingExample[] yArr = y.ToArray();
            
            if (xArr.Length != yArr.Length)
            {
                return false;
            }

            return !xArr.Where((t, i) => t.Value != yArr[i].Value).Any();
        }

        public int GetHashCode(IEnumerable<SortingExample> obj)
        {
            return obj.Aggregate(0, (current, t) => HashCode.Combine(current, t.Value.GetHashCode()));
        }
    }
    
    [Fact]
    public void TestRefreshOrder1()
    {
        List<SortingExample> values =
        [
            new(0, "A"),
            new(1, "B"),
            new(3, "D"),
            new(4, "E")
        ];
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "E")
        ];
        Sort.RefreshOrder(values);
        Assert.Equal(expected, values, new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestRefreshOrder2()
    {
        List<SortingExample> values =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "E")
        ];
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "E")
        ];
        Sort.RefreshOrder(values);
        Assert.Equal(expected, values, new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestRefreshOrder3()
    {
        List<SortingExample> values =
        [
            new(-1, "A"),
            new(1, "B"),
            new(2, "D"),
            new(30, "E")
        ];
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "E")
        ];
        Sort.RefreshOrder(values);
        Assert.Equal(expected, values, new SortingExampleListComparer());
    }
    
    // Reorder tests
    private readonly List<SortingExample> _values =
    [
        new(0, "A"),
        new(1, "B"),
        new(2, "C"),
        new(3, "D")
    ];
    
    [Fact]
    public void TestReorder00()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "C"),
            new(3, "D"),
        ];

        Sort.Reorder(_values, 0, 0);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder01()
    {
        List<SortingExample> expected =
        [
            new(0, "B"),
            new(1, "A"),
            new(2, "C"),
            new(3, "D"),
        ];

        Sort.Reorder(_values, 0, 1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder02()
    {
        List<SortingExample> expected =
        [
            new(0, "B"),
            new(1, "C"),
            new(2, "A"),
            new(3, "D"),
        ];

        Sort.Reorder(_values, 0, 2);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder03()
    {
        List<SortingExample> expected =
        [
            new(0, "B"),
            new(1, "C"),
            new(2, "D"),
            new(3, "A"),
        ];

        Sort.Reorder(_values, 0, 3);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder10()
    {
        List<SortingExample> expected =
        [
            new(0, "B"),
            new(1, "A"),
            new(2, "C"),
            new(3, "D"),
        ];

        Sort.Reorder(_values, 1, 0);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder11()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "C"),
            new(3, "D"),
        ];

        Sort.Reorder(_values, 1, 1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder12()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "C"),
            new(2, "B"),
            new(3, "D"),
        ];

        Sort.Reorder(_values, 1, 2);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder13()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "C"),
            new(2, "D"),
            new(3, "B"),
        ];

        Sort.Reorder(_values, 1, 3);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder2N()
    {
        List<SortingExample> expected =
        [
            new(0, "C"),
            new(1, "A"),
            new(2, "B"),
            new(3, "D")
        ];

        Sort.Reorder(_values, 2, -1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder20()
    {
        List<SortingExample> expected =
        [
            new(0, "C"),
            new(1, "A"),
            new(2, "B"),
            new(3, "D")
        ];

        Sort.Reorder(_values, 2, 0);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder21()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "C"),
            new(2, "B"),
            new(3, "D")
        ];

        Sort.Reorder(_values, 2, 1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder22()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "C"),
            new(3, "D")
        ];

        Sort.Reorder(_values, 2, 2);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder23()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "C")
        ];

        Sort.Reorder(_values, 2, 3);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder24()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "C")
        ];

        Sort.Reorder(_values, 2, 4);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder29()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "C")
        ];

        Sort.Reorder(_values, 2, 9);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder30()
    {
        List<SortingExample> expected =
        [
            new(0, "D"),
            new(1, "A"),
            new(2, "B"),
            new(3, "C"),
        ];

        Sort.Reorder(_values, 3, 0);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder31()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "D"),
            new(2, "B"),
            new(3, "C"),
        ];

        Sort.Reorder(_values, 3, 1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder32()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "C"),
        ];

        Sort.Reorder(_values, 3, 2);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
    
    [Fact]
    public void TestReorder33()
    {
        List<SortingExample> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "C"),
            new(3, "D"),
        ];

        Sort.Reorder(_values, 3, 3);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new SortingExampleListComparer());
    }
}