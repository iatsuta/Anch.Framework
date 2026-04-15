namespace CommonFramework;

public static class ActionExtensions
{
    public static Func<object?> ToDefaultFunc(this Action action) => () =>
    {
        action();
        return null;
    };

    public static Func<T, object?> ToDefaultFunc<T>(this Action<T> action) => a =>
    {
        action(a);
        return null;
    };

    public static Func<T1, T2, object?> ToDefaultFunc<T1, T2>(this Action<T1, T2> action) => (a1, a2) =>
    {
        action(a1, a2);
        return null;
    };
}