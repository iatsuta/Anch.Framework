namespace Anch.SecuritySystem.Builders._Factory;

/// <summary>Creates a filter for a domain type and a security rule.</summary>
public interface IFilterFactory<TDomainObject, TFilter>
{
    Task<TFilter> CreateFilterAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, SecurityPath<TDomainObject> securityPath, CancellationToken ct);
}
