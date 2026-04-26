using Anch.SecuritySystem.Builders._Factory;
using Anch.SecuritySystem.Builders._Filter;

namespace Anch.SecuritySystem.Builders.MixedBuilder;

public class SecurityFilterFactory<TDomainObject>(
    ISecurityFilterFactory<TDomainObject> queryFactory,
    ISecurityFilterFactory<TDomainObject> hasAccessFactory)
    : ISecurityFilterFactory<TDomainObject>
{
    public async ValueTask<SecurityFilterInfo<TDomainObject>> CreateFilterAsync(DomainSecurityRule.RoleBaseSecurityRule securityRule,
        SecurityPath<TDomainObject> securityPath, CancellationToken cancellationToken)
    {
        return new SecurityFilterInfo<TDomainObject>(
            (await queryFactory.CreateFilterAsync(securityRule, securityPath, cancellationToken)).InjectFunc,
            (await hasAccessFactory.CreateFilterAsync(securityRule, securityPath, cancellationToken)).HasAccessFunc);
    }
}