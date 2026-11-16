using System.Collections.Immutable;

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>The registry of configured roles.</summary>
public interface ISecurityRoleSource
{
    /// <summary>All registered roles.</summary>
    ImmutableArray<FullSecurityRole> SecurityRoles { get; }

    /// <summary>Returns the registration of a role.</summary>
    /// <param name="securityRole">The role.</param>
    FullSecurityRole GetSecurityRole(SecurityRole securityRole);

    /// <summary>Returns the registration of a role by its name.</summary>
    /// <param name="name">The role name.</param>
    FullSecurityRole GetSecurityRole(string name);

    /// <summary>Returns the registration of a role by its identifier.</summary>
    /// <param name="identity">The role identifier.</param>
    FullSecurityRole GetSecurityRole(SecurityIdentity identity);

    /// <summary>Returns the roles that are not virtual.</summary>
    IEnumerable<FullSecurityRole> GetRealRoles();
}
