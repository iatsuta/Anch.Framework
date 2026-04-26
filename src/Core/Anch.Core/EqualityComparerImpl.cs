namespace Anch.Core;

public class EqualityComparerImpl<T>(Func<T, T, bool> equalsFunc, Func<T, int>? getHashFunc = null) : EqualityComparer<T>
{
    public override bool Equals(T? x, T? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (x is null || y is null) return false;

        return equalsFunc(x, y);
    }

    public override int GetHashCode(T obj)
    {
        return getHashFunc.Maybe(v => v(obj));
    }

    public static EqualityComparerImpl<T> Create<TKey>(Func<T, TKey> keySelector)
    {
        var keyComparer = EqualityComparer<TKey>.Default;

        return new EqualityComparerImpl<T>((v1, v2) => keyComparer.Equals(keySelector(v1), keySelector(v2)));
    }
}