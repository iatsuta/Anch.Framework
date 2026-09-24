namespace Anch.GenericQueryable.Fetching;

public record CompositeFetchRule<TSource>(FetchRule<TSource> Left, FetchRule<TSource> Right) : FetchRule<TSource>;