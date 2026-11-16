using System.Linq.Expressions;

namespace Anch.SecuritySystem.VirtualPermission;

/// <summary>Describes one role granted by a virtual permission, with its optional filter.</summary>
public abstract record VirtualPermissionSecurityRoleItemBindingInfo
{
    public abstract Type PermissionType { get; }

    public required SecurityRole SecurityRole { get; init; }
}

/// <summary>Describes one role granted by a virtual permission, with its optional filter.</summary>
public record VirtualPermissionSecurityRoleItemBindingInfo<TPermission> : VirtualPermissionSecurityRoleItemBindingInfo
{
    public override Type PermissionType { get; } = typeof(TPermission);


    public Func<IServiceProvider, Expression<Func<TPermission, bool>>> Filter { get; init; } = _ => _ => true;
}