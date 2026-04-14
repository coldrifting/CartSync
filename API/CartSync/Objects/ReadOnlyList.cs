using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace CartSync.Objects;

/// <summary>
/// An Immutable list with value semantics for built-in equality checking
/// </summary>
[DebuggerDisplay("Count = {Count}")]
[DebuggerTypeProxy(typeof(ICollection<>))]
[CollectionBuilder(typeof(ReadOnlyListBuilder), nameof(ReadOnlyListBuilder.Create))]
public class ReadOnlyList<T> : IImmutableList<T>, IEquatable<IImmutableList<T>>
{
    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    private readonly IImmutableList<T> _list;
    
    public ReadOnlyList()
    {
        _list = ImmutableList<T>.Empty;
    }
    
    public ReadOnlyList(IEnumerable<T> items)
    {
        _list = items.ToImmutableList();
    }
    
    internal static ReadOnlyList<T> Create(ReadOnlyList<T> values) => new(values);
    
    #region IImutableList implementation
    public T this[int index] => _list[index];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public int Count => _list.Count;

    public IImmutableList<T> Add(T value) => new ReadOnlyList<T>(_list.Add(value));
    public IImmutableList<T> AddRange(IEnumerable<T> items) => new ReadOnlyList<T>(_list.AddRange(items));
    public IImmutableList<T> Clear() => new ReadOnlyList<T>(_list.Clear());
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    public int IndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) => _list.IndexOf(item, index, count, equalityComparer);
    public IImmutableList<T> Insert(int index, T element) => new ReadOnlyList<T>(_list.Insert(index, element));
    public IImmutableList<T> InsertRange(int index, IEnumerable<T> items) => new ReadOnlyList<T>(_list.InsertRange(index, items));
    public int LastIndexOf(T item, int index, int count, IEqualityComparer<T>? equalityComparer) => _list.LastIndexOf(item, index, count, equalityComparer);
    public IImmutableList<T> Remove(T value, IEqualityComparer<T>? equalityComparer) => new ReadOnlyList<T>(_list.Remove(value, equalityComparer));
    public IImmutableList<T> RemoveAll(Predicate<T> match) => new ReadOnlyList<T>(_list.RemoveAll(match));
    public IImmutableList<T> RemoveAt(int index) => new ReadOnlyList<T>(_list.RemoveAt(index));
    public IImmutableList<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T>? equalityComparer) => new ReadOnlyList<T>(_list.RemoveRange(items, equalityComparer));
    public IImmutableList<T> RemoveRange(int index, int count) => new ReadOnlyList<T>(_list.RemoveRange(index, count));
    public IImmutableList<T> Replace(T oldValue, T newValue, IEqualityComparer<T>? equalityComparer) => new ReadOnlyList<T>(_list.Replace(oldValue, newValue, equalityComparer));
    public IImmutableList<T> SetItem(int index, T value) => _list.SetItem(index, value);
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    #endregion

    public override bool Equals(object? obj) => Equals(obj as IImmutableList<T>);
    public bool Equals(IImmutableList<T>? other) => this.SequenceEqual(other ?? ImmutableList<T>.Empty);
    public override int GetHashCode()  
    {
        unchecked
        {
            return this.Aggregate(19, (h, i) =>
            {
                if (i != null)
                {
                    return h * 19 + i.GetHashCode();
                }

                return h * 19 + 0;
            });
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new();
        
        builder.Append("ReadOnlyList[");

        foreach (T item in _list)
        {
            builder.Append(item);
            builder.Append(", ");
        }
        
        builder.Length--;
        builder.Append(']');
        
        return builder.ToString();
    }
}

public static class ReadOnlyListBuilder
{
    public static ReadOnlyList<T> Create<T>(ReadOnlySpan<T> values)
    {
        List<T> list = new(values.Length);
        list.AddRange(values);
        return new ReadOnlyList<T>(list);
    }
}

public static class Ex
{
    public static ReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> enumerable) =>
        new(enumerable.ToImmutableList());

    public static async Task<ReadOnlyList<TSource>> ToReadOnlyListAsync<TSource>(
        this IQueryable<TSource> source,
        CancellationToken cancellationToken = default)
    {
        List<TSource> list = [];
        await foreach (TSource element in source.AsAsyncEnumerable().WithCancellation(cancellationToken)
                           .ConfigureAwait(false))
        {
            list.Add(element);
        }

        return list.ToReadOnlyList();
    }
}