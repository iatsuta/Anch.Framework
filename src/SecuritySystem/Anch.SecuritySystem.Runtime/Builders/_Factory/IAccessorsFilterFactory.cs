using Anch.SecuritySystem.Builders._Filter;

namespace Anch.SecuritySystem.Builders._Factory;

/// <summary>Creates the computation of who has access to an object.</summary>
public interface IAccessorsFilterFactory<TDomainObject> : IFilterFactory<TDomainObject, AccessorsFilterInfo<TDomainObject>>;
