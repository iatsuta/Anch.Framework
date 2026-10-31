namespace Anch.GenericQueryable.Fetching;

/// <summary>Combines two fetch rules; both are applied.</summary>
/// <param name="Left">The first rule.</param>
/// <param name="Right">The second rule.</param>
public record CompositeFetchRule<TSource>(FetchRule<TSource> Left, FetchRule<TSource> Right) : FetchRule<TSource>;