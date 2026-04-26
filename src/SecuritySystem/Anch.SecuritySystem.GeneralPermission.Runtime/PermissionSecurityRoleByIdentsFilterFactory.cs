using System.Linq.Expressions;

using Anch.Core;
using Anch.IdentitySource;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

public class PermissionSecurityRoleByIdentsFilterFactory<TPermission>(
    IServiceProxyFactory serviceProxyFactory,
    GeneralPermissionBindingInfo<TPermission> generalBindingInfo,
    IIdentityInfoSource identityInfoSource) : IPermissionSecurityRoleByIdentsFilterFactory<TPermission>
{
    private readonly Lazy<IPermissionSecurityRoleByIdentsFilterFactory<TPermission>> lazyInnerService = new(() =>
    {
        var securityRoleIdentityInfo = identityInfoSource.GetIdentityInfo(generalBindingInfo.SecurityRoleType);

        var innerServiceType = typeof(PermissionSecurityRoleByIdentsFilterFactory<,,>).MakeGenericType(
            generalBindingInfo.PermissionType,
            generalBindingInfo.SecurityRoleType,
            securityRoleIdentityInfo.IdentityType);

        return serviceProxyFactory.Create<IPermissionSecurityRoleByIdentsFilterFactory<TPermission>>(innerServiceType);
    });

    public Expression<Func<TPermission, bool>> CreateFilter(Type identType, Array idents) =>
        this.lazyInnerService.Value.CreateFilter(identType, idents);
}

public class PermissionSecurityRoleByIdentsFilterFactory<TPermission, TSecurityRole, TSecurityRoleIdent>(
    GeneralPermissionBindingInfo<TPermission, TSecurityRole> generalBindingInfo,
    ISecurityIdentityConverter<TSecurityRoleIdent> securityIdentityConverter,
    IIdentityInfo<TSecurityRole, TSecurityRoleIdent> identityInfo) : IPermissionSecurityRoleByIdentsFilterFactory<TPermission>
    where TSecurityRoleIdent : notnull
{
    public Expression<Func<TPermission, bool>> CreateFilter(Type identType, Array idents)
    {
        return new Func<Ignore[], Expression<Func<TPermission, bool>>>(this.CreateFilter)
            .CreateGenericMethod(identType)
            .Invoke<Expression<Func<TPermission, bool>>>(this, idents);
    }

    private Expression<Func<TPermission, bool>> CreateFilter<TIdent>(TIdent[] idents)
        where TIdent : notnull
    {
        var convertedIdents = idents.Select(ident => securityIdentityConverter.Convert(TypedSecurityIdentity.Create(ident)).Id).ToList();

        var containsFilter = identityInfo.CreateFilter(convertedIdents);

        return generalBindingInfo.SecurityRole.Path.Select(containsFilter);
    }
}