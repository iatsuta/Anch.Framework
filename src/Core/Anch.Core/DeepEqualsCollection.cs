using System.Collections.Immutable;

namespace Anch.Core;

public class DeepEqualsCollection<T>(ImmutableArray<T> baseSource, IEqualityComparer<T> comparer)
    : IReadOnlyList<T>, IEquatable<DeepEqualsCollection<T>>
{
    private int? hashCode;

    public DeepEqualsCollection(IEnumerable<T> baseSource, IEqualityComparer<T>? comparer = null)
        : this([..baseSource], comparer ?? EqualityComparer<T>.Default)
    {
    }

    public int Count => baseSource.Length;

    public T this[int index] => baseSource[index];

    public IEnumerator<T> GetEnumerator() => ((IEnumerable<T>)baseSource).GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => this.GetEnumerator();

    public bool Equals(DeepEqualsCollection<T>? other) =>

        object.ReferenceEquals(this, other)

        || (other is not null && baseSource.SequenceEqual(other, comparer));

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
        return object.Equals(col1, col2);
    }

    public static bool operator !=(DeepEqualsCollection<T>? col1, DeepEqualsCollection<T>? col2)
    {
        return !(col1 == col2);
    }


    public static implicit operator DeepEqualsCollection<T>(T[] source) => new (source);
}

public static class DeepEqualsCollection
{
    public static DeepEqualsCollection<T> Create<T>(IEnumerable<T> source) => Create(source, null);

    public static DeepEqualsCollection<T> Create<T>(IEnumerable<T> source, IEqualityComparer<T>? comparer) => new(source, comparer);
}