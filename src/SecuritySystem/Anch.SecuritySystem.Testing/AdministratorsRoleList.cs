using System.Collections.Immutable;

namespace Anch.SecuritySystem.Testing;

public record AdministratorsRoleList(ImmutableArray<SecurityRole> Roles)
{
    public static AdministratorsRoleList Default { get; } = new([SecurityRole.Administrator, SecurityRole.SystemIntegration]);
}