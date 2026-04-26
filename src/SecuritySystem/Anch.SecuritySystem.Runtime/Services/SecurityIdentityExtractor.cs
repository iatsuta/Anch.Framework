using System.Linq.Expressions;
using Anch.Core;
using Anch.IdentitySource;

namespace Anch.SecuritySystem.Services;

public class SecurityIdentityManager<TDomainObject>(IServiceProxyFactory serviceProxyFactory, IIdentityInfoSource identityInfoSource)
    : ISecurityIdentityManager<TDomainObject>
{
    private readonly Lazy<ISecurityIdentityManager<TDomainObject>> lazyInnerService = new(() =>
    {
        var identityInfo = identityInfoSource.GetIdentityInfo<TDomainObject>();

        var innerServiceType = typeof(SecurityIdentityManager<,>).MakeGenericType(identityInfo.DomainObjectType, identityInfo.IdentityType);

        return serviceProxyFactory.Create<ISecurityIdentityManager<TDomainObject>>(innerServiceType);
    });

    public ISecurityIdentityConverter Converter => this.lazyInnerService.Value.Converter;

    public Expression<Func<TDomainObject, TypedSecurityIdentity>> SecurityIdentityExpression => this.lazyInnerService.Value.SecurityIdentityExpression;

    public TypedSecurityIdentity GetIdentity(TDomainObject domainObject) => this.lazyInnerService.Value.GetIdentity(domainObject);

    public void SetIdentity(TDomainObject domainObject, SecurityIdentity securityIdentity) =>
        this.lazyInnerService.Value.SetIdentity(domainObject, securityIdentity);
}

public class SecurityIdentityManager<TDomainObject, TDomainObjectIdent>(
    ISecurityIdentityConverter<TDomainObjectIdent> converter,
    IIdentityInfo<TDomainObject, TDomainObjectIdent> identityInfo)
    : ISecurityIdentityManager<TDomainObject>
    where TDomainObjectIdent : notnull
{
    public ISecurityIdentityConverter Converter { get; } = converter;

    public Expression<Func<TDomainObject, TypedSecurityIdentity>> SecurityIdentityExpression { get; } =
        identityInfo.Id.Path.Select(
            converter.GetConvertExpression<TDomainObjectIdent>().Select(id => (TypedSecurityIdentity)new TypedSecurityIdentity<TDomainObjectIdent>(id)));

    public TypedSecurityIdentity GetIdentity(TDomainObject domainObject) => TypedSecurityIdentity.Create(identityInfo.Id.Getter(domainObject));

    public void SetIdentity(TDomainObject domainObject, SecurityIdentity securityIdentity) =>
        identityInfo.Id.Setter(domainObject, converter.Convert(securityIdentity).Id);
}