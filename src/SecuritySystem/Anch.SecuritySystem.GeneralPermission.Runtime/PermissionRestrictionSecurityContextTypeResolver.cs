using Anch.Core;
using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.GeneralPermission;

/// <summary>Finds the security context type that a restriction entity refers to.</summary>
public class PermissionRestrictionSecurityContextTypeResolver<TPermissionRestriction, TSecurityContextType>(
    GeneralPermissionRestrictionBindingInfo<TPermissionRestriction, TSecurityContextType> restrictionBindingInfo,
    ISecurityIdentityManager<TSecurityContextType> securityContextTypeSecurityIdentityManager,
    ISecurityContextInfoSource securityContextInfoSource) : IPermissionRestrictionSecurityContextTypeResolver<TPermissionRestriction>
{
    public Type Resolve(TPermissionRestriction permissionRestriction)
    {
        return restrictionBindingInfo
            .SecurityContextType.Getter(permissionRestriction)
            .Pipe(securityContextTypeSecurityIdentityManager.GetIdentity)
            .Pipe(identity => securityContextInfoSource.GetSecurityContextInfo(identity).Type);
    }
}