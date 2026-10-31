

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>A role together with its registration data.</summary>
/// <param name="name">The role name.</param>
/// <param name="information">Its registration data.</param>
public class FullSecurityRole(string name, SecurityRoleInfo information) : SecurityRole(name)
{
    /// <summary>The registration data.</summary>
    public SecurityRoleInfo Information { get; } = information;

    /// <summary>The role identifier.</summary>
    public TypedSecurityIdentity Identity => this.Information.Identity;
}
