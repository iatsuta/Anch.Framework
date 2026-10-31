namespace Anch.SecuritySystem.Builders._Filter;

/// <summary>The access filter: how to restrict a query and how to check one object.</summary>
public record SecurityFilterInfo<TDomainObject>(
    Func<IQueryable<TDomainObject>, IQueryable<TDomainObject>> InjectFunc,
    Func<TDomainObject, bool> HasAccessFunc);
