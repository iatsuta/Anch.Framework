namespace Anch.SecuritySystem.Builders._Filter;

/// <summary>The computation of who has access to an object.</summary>
public record AccessorsFilterInfo<TDomainObject>(Func<TDomainObject, IAsyncEnumerable<string>> GetAccessorsFunc);
