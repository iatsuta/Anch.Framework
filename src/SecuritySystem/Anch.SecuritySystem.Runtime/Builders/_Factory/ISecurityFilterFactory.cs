using Anch.SecuritySystem.Builders._Filter;

namespace Anch.SecuritySystem.Builders._Factory;

public interface ISecurityFilterFactory<TDomainObject> : IFilterFactory<TDomainObject, SecurityFilterInfo<TDomainObject>>;
