using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

public interface ISecurityIdentityManager<TDomainObject>
{
    ISecurityIdentityConverter Converter { get; }

    Expression<Func<TDomainObject, TypedSecurityIdentity>> SecurityIdentityExpression { get; }

    TypedSecurityIdentity GetIdentity(TDomainObject domainObject);

    void SetIdentity(TDomainObject domainObject, SecurityIdentity securityIdentity);
}