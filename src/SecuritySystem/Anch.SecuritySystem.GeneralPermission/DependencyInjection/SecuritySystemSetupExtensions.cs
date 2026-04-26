using System.Linq.Expressions;
using Anch.Core;
using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;

namespace Anch.SecuritySystem.GeneralPermission.DependencyInjection;

public static class SecuritySystemSetupExtensions
{
    extension(ISecuritySystemSetup securitySystemSetup)
    {
        public ISecuritySystemSetup AddGeneralPermission<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction, TSecurityContextType,
            TSecurityContextObjectIdent>(
            PropertyAccessors<TPermission, TPrincipal> principalAccessors,
            PropertyAccessors<TPermission, TSecurityRole> securityRoleAccessors,
            PropertyAccessors<TPermissionRestriction, TPermission> permissionAccessors,
            PropertyAccessors<TPermissionRestriction, TSecurityContextType> securityContextTypeAccessors,
            PropertyAccessors<TPermissionRestriction, TSecurityContextObjectIdent> securityContextObjectIdAccessors,
            Action<IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction>>? setupAction = null)
            where TPrincipal : class
            where TPermission : class, new()
            where TSecurityRole : class, new()
            where TPermissionRestriction : class, new()
            where TSecurityContextType : class, new()
            where TSecurityContextObjectIdent : notnull =>
            securitySystemSetup
                .Initialize<ISecuritySystemSetup,
                    GeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction, TSecurityContextType,
                        TSecurityContextObjectIdent>>
                (new (principalAccessors, securityRoleAccessors, permissionAccessors, securityContextTypeAccessors, securityContextObjectIdAccessors),
                    setupAction);

        public ISecuritySystemSetup AddGeneralPermission<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction, TSecurityContextType,
            TSecurityContextObjectIdent>(
            Expression<Func<TPermission, TPrincipal>> principalPath,
            Expression<Func<TPermission, TSecurityRole>> securityRolePath,
            Expression<Func<TPermissionRestriction, TPermission>> permissionPath,
            Expression<Func<TPermissionRestriction, TSecurityContextType>> securityContextTypePath,
            Expression<Func<TPermissionRestriction, TSecurityContextObjectIdent>> securityContextObjectIdPath,
            Action<IGeneralPermissionSetup<TPrincipal, TPermission, TSecurityRole, TPermissionRestriction>>? setupAction = null)
            where TPrincipal : class
            where TPermission : class, new()
            where TSecurityRole : class, new()
            where TPermissionRestriction : class, new()
            where TSecurityContextType : class, new()
            where TSecurityContextObjectIdent : notnull =>
            securitySystemSetup.AddGeneralPermission(
                principalPath.ToPropertyAccessors(),
                securityRolePath.ToPropertyAccessors(),
                permissionPath.ToPropertyAccessors(),
                securityContextTypePath.ToPropertyAccessors(),
                securityContextObjectIdPath.ToPropertyAccessors(),
                setupAction);
    }
}