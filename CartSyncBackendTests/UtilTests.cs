using CartSyncBackend.Database.Interfaces;
using CartSyncBackend.Utils;

namespace CartSyncBackendTests;

public class UtilTests
{
    private class TestClass(int id, string value) : ISortable
    {
        public int SortOrder { get; set; } = id;
        public string Value { get; } = value;

        public override bool Equals(object? obj)
        {
            if (obj is not TestClass test)
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
    
    private class TestClassListComparer : IEqualityComparer<IEnumerable<TestClass>>
    {
        public bool Equals(IEnumerable<TestClass>? x, IEnumerable<TestClass>? y)
        {
            if (x == null || y == null)
            {
                return x == null && y == null;
            }
            
            TestClass[] xArr = x.ToArray();
            TestClass[] yArr = y.ToArray();
            
            if (xArr.Length != yArr.Length)
            {
                return false;
            }

            return !xArr.Where((t, i) => t.Value != yArr[i].Value).Any();
        }

        public int GetHashCode(IEnumerable<TestClass> obj)
        {
            return obj.Aggregate(0, (current, t) => HashCode.Combine(current, t.Value.GetHashCode()));
        }
    }
    
    [Fact]
    public void TestRefreshOrder1()
    {
        List<TestClass> values =
        [
            new(0, "A"),
            new(1, "B"),
            new(3, "D"),
            new(4, "E")
        ];
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "E")
        ];
        values.RefreshOrder();
        Assert.Equal(expected, values, new TestClassListComparer());
    }
    
    [Fact]
    public void TestRefreshOrder2()
    {
        List<TestClass> values =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "E")
        ];
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "E")
        ];
        values.RefreshOrder();
        Assert.Equal(expected, values, new TestClassListComparer());
    }
    
    [Fact]
    public void TestRefreshOrder3()
    {
        List<TestClass> values =
        [
            new(-1, "A"),
            new(1, "B"),
            new(2, "D"),
            new(30, "E")
        ];
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "E")
        ];
        values.RefreshOrder();
        Assert.Equal(expected, values, new TestClassListComparer());
    }
    
    // Reorder tests
    private readonly List<TestClass> _values =
    [
        new(0, "A"),
        new(1, "B"),
        new(2, "C"),
        new(3, "D")
    ];
    
    [Fact]
    public void TestReorder00()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "C"),
            new(3, "D"),
        ];

        _values.Reorder(0, 0);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder01()
    {
        List<TestClass> expected =
        [
            new(0, "B"),
            new(1, "A"),
            new(2, "C"),
            new(3, "D"),
        ];

        _values.Reorder(0, 1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder02()
    {
        List<TestClass> expected =
        [
            new(0, "B"),
            new(1, "C"),
            new(2, "A"),
            new(3, "D"),
        ];

        _values.Reorder(0, 2);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder03()
    {
        List<TestClass> expected =
        [
            new(0, "B"),
            new(1, "C"),
            new(2, "D"),
            new(3, "A"),
        ];

        _values.Reorder(0, 3);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder10()
    {
        List<TestClass> expected =
        [
            new(0, "B"),
            new(1, "A"),
            new(2, "C"),
            new(3, "D"),
        ];

        _values.Reorder(1, 0);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder11()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "C"),
            new(3, "D"),
        ];

        _values.Reorder(1, 1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder12()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "C"),
            new(2, "B"),
            new(3, "D"),
        ];

        _values.Reorder(1, 2);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder13()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "C"),
            new(2, "D"),
            new(3, "B"),
        ];

        _values.Reorder(1, 3);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder2N()
    {
        List<TestClass> expected =
        [
            new(0, "C"),
            new(1, "A"),
            new(2, "B"),
            new(3, "D")
        ];

        _values.Reorder(2, -1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder20()
    {
        List<TestClass> expected =
        [
            new(0, "C"),
            new(1, "A"),
            new(2, "B"),
            new(3, "D")
        ];

        _values.Reorder(2, 0);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder21()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "C"),
            new(2, "B"),
            new(3, "D")
        ];

        _values.Reorder(2, 1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder22()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "C"),
            new(3, "D")
        ];

        _values.Reorder(2, 2);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder23()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "C")
        ];

        _values.Reorder(2, 3);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder24()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "C")
        ];

        _values.Reorder(2, 4);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder29()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "C")
        ];

        _values.Reorder(2, 9);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder30()
    {
        List<TestClass> expected =
        [
            new(0, "D"),
            new(1, "A"),
            new(2, "B"),
            new(3, "C"),
        ];

        _values.Reorder(3, 0);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder31()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "D"),
            new(2, "B"),
            new(3, "C"),
        ];

        _values.Reorder(3, 1);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder32()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "D"),
            new(3, "C"),
        ];

        _values.Reorder(3, 2);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
    
    [Fact]
    public void TestReorder33()
    {
        List<TestClass> expected =
        [
            new(0, "A"),
            new(1, "B"),
            new(2, "C"),
            new(3, "D"),
        ];

        _values.Reorder(3, 3);
        Assert.Equal(expected, _values.OrderBy(i => i.SortOrder).ToList(), new TestClassListComparer());
    }
}