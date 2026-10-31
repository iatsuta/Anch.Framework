using System.Collections.Immutable;

namespace Anch.Core;

/// <summary>An immutable list that is equal to another one when it has equal elements in the same order. Use it as a member of records, so that the records compare by content.</summary>
/// <param name="baseSource">The elements.</param>
/// <param name="comparer">Compares the elements.</param>
/// <typeparam name="T">The element type.</typeparam>
public class DeepEqualsCollection<T>(ImmutableArray<T> baseSource, IEqualityComparer<T> comparer)
    : IReadOnlyList<T>, IEquatable<DeepEqualsCollection<T>>
{
    private int? hashCode;

    /// <summary>Copies the elements.</summary>
    /// <param name="baseSource">The elements.</param>
    /// <param name="comparer">Compares the elements; <c>null</c> for the default comparer.</param>
    public DeepEqualsCollection(IEnumerable<T> baseSource, IEqualityComparer<T>? comparer = null)
        : this([.. baseSource], comparer ?? EqualityComparer<T>.Default)
    {
    }

    public int Count => baseSource.Length;

    public T this[int index] => baseSource[index];

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)baseSource).GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

    public bool Equals(DeepEqualsCollection<T>? other) =>

        ReferenceEquals(this, other)

        || (other is not null
            && this.GetHashCode() == other.GetHashCode()
            && this.Count == other.Count
            && baseSource.SequenceEqual(other, comparer));

    public override bool Equals(object? obj) => this.Equals(obj as DeepEqualsCollection<T>);

    public override int GetHashCode()
    {
        return this.hashCode ??= this.ComputeHashCode();
    }

    private int ComputeHashCode()
    {
        var hash = new HashCode();

        hash.Add(baseSource.Length);

        foreach (var expr in baseSource)
            hash.Add(expr, comparer);

        return hash.ToHashCode();
    }

    public static bool operator ==(DeepEqualsCollection<T>? col1, DeepEqualsCollection<T>? col2)
    {
        return Equals(col1, col2);
    }

    public static bool operator !=(DeepEqualsCollection<T>? col1, DeepEqualsCollection<T>? col2)
    {
        return !(col1 == col2);
    }


    /// <summary>Wraps an array.</summary>
    /// <param name="source">The elements.</param>
    public static implicit operator DeepEqualsCollection<T>(T[] source) => new(source);
}

/// <summary>Factory methods of <see cref="DeepEqualsCollection{T}"/>.</summary>
public static class DeepEqualsCollection
{
    /// <summary>Creates a collection with the default element comparer.</summary>
    /// <param name="source">The elements.</param>
    /// <typeparam name="T">The element type.</typeparam>
    public static DeepEqualsCollection<T> Create<T>(IEnumerable<T> source) => Create(source, null);

    /// <summary>Creates a collection.</summary>
    /// <param name="source">The elements.</param>
    /// <param name="comparer">Compares the elements; <c>null</c> for the default comparer.</param>
    /// <typeparam name="T">The element type.</typeparam>
    public static DeepEqualsCollection<T> Create<T>(IEnumerable<T> source, IEqualityComparer<T>? comparer) => new(source, comparer);
}