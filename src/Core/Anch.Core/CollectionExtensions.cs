namespace Anch.Core;

public static class CollectionExtensions
{
    public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> args)
    {
        args.Foreach(source.Add);
    }

    public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> items)
    {
        items.Foreach(item => source.Remove(item));
    }
}