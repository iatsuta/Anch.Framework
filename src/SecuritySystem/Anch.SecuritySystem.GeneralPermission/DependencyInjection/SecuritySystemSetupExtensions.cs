using System.Linq.Expressions;

using Anch.Core;
using Anch.DependencyInjection;
using Anch.SecuritySystem.DependencyInjection;

namespace Anch.SecuritySystem.GeneralPermission.DependencyInjection;

/// <summary>Registration of the GeneralPermission source.</summary>
public static class SecuritySystemSetupExtensions
{
    extension(ISecuritySystemSetup securitySystemSetup)
    {
        /// <summary>
        /// Adds a permission source whose permissions are stored as entities of the application: a principal, a permission (principal + role)
        /// and permission restrictions (security context type + object identifier).
        /// </summary>
        /// <param name="principalAccessors">The principal of a permission.</param>
        /// <param name="securityRoleAccessors">The role of a permission.</param>
        /// <param name="permissionAccessors">The permission of a restriction.</param>
        /// <param name="securityContextTypeAccessors">The security context type of a restriction.</param>
        /// <param name="securityContextObjectIdAccessors">The security context object identifier of a restriction.</param>
        /// <param name="setupAction">Optional settings: period, comment, delegation, read-only mode.</param>
        /// <typeparam name="TPrincipal">The principal entity type.</typeparam>
        /// <typeparam name="TPermission">The permission entity type.</typeparam>
        /// <typeparam name="TSecurityRole">The role entity type.</typeparam>
        /// <typeparam name="TPermissionRestriction">The permission restriction entity type.</typeparam>
        /// <typeparam name="TSecurityContextType">The entity type that describes a security context type.</typeparam>
        /// <typeparam name="TSecurityContextObjectIdent">The type of the security context object identifier.</typeparam>
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
                (new(principalAccessors, securityRoleAccessors, permissionAccessors, securityContextTypeAccessors, securityContextObjectIdAccessors),
                    setupAction);

        /// <summary>
        /// Adds a permission source whose permissions are stored as entities of the application: a principal, a permission (principal + role)
        /// and permission restrictions (security context type + object identifier).
        /// </summary>
        /// <param name="principalPath">Selects the principal of a permission.</param>
        /// <param name="securityRolePath">Selects the role of a permission.</param>
        /// <param name="permissionPath">Selects the permission of a restriction.</param>
        /// <param name="securityContextTypePath">Selects the security context type of a restriction.</param>
        /// <param name="securityContextObjectIdPath">Selects the security context object identifier of a restriction.</param>
        /// <param name="setupAction">Optional settings: period, comment, delegation, read-only mode.</param>
        /// <typeparam name="TPrincipal">The principal entity type.</typeparam>
        /// <typeparam name="TPermission">The permission entity type.</typeparam>
        /// <typeparam name="TSecurityRole">The role entity type.</typeparam>
        /// <typeparam name="TPermissionRestriction">The permission restriction entity type.</typeparam>
        /// <typeparam name="TSecurityContextType">The entity type that describes a security context type.</typeparam>
        /// <typeparam name="TSecurityContextObjectIdent">The type of the security context object identifier.</typeparam>
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