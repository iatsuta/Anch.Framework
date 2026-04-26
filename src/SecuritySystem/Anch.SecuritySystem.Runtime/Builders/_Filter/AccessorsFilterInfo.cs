namespace Anch.SecuritySystem.Builders._Filter;

public record AccessorsFilterInfo<TDomainObject>(Func<TDomainObject, IAsyncEnumerable<string>> GetAccessorsFunc);
