using Anch.SecuritySystem.Builders._Filter;

namespace Anch.SecuritySystem.Builders._Factory;

/// <summary>Creates the access filter of a domain type: a query transformation and a check of a single object.</summary>
public interface ISecurityFilterFactory<TDomainObject> : IFilterFactory<TDomainObject, SecurityFilterInfo<TDomainObject>>;
