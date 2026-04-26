namespace Anch.Core;

public class ArrayComparer<T>(IEqualityComparer<T> itemComparer) : IEqualityComparer<T[]>
{
    public bool Equals(T[]? x, T[]? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return x.SequenceEqual(y, itemComparer);
    }

    public int GetHashCode(T[] array)
    {
        return array.Length;
    }


    public static readonly ArrayComparer<T> Default = new(EqualityComparer<T>.Default);
}