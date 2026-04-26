using Anch.SecuritySystem.Builders._Filter;

namespace Anch.SecuritySystem.Builders._Factory;

public interface IAccessorsFilterFactory<TDomainObject> : IFilterFactory<TDomainObject, AccessorsFilterInfo<TDomainObject>>;
