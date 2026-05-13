namespace Anch.SecuritySystem.Builders._Factory;

public interface IFilterFactory<TDomainObject, TFilter>
{
    Task<TFilter> CreateFilterAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule, SecurityPath<TDomainObject> securityPath, CancellationToken cancellationToken);
}
