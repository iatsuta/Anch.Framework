using System.Collections.Immutable;

namespace Anch.SecuritySystem.Testing;

/// <summary>The roles that count as administrator roles in tests.</summary>
public record AdministratorsRoleList(ImmutableArray<SecurityRole> Roles)
{
    public static AdministratorsRoleList Default { get; } = new([SecurityRole.Administrator, SecurityRole.SystemIntegration]);
}