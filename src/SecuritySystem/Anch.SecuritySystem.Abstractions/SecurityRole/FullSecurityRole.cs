

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public class FullSecurityRole(string name, SecurityRoleInfo information) : SecurityRole(name)
{
    public SecurityRoleInfo Information { get; } = information;

    public TypedSecurityIdentity Identity => this.Information.Identity;
}
