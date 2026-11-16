using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Services;

/// <summary>Extracts the identifier from the data of a stored principal.</summary>
public interface IPrincipalDataSecurityIdentityManager
{
    /// <summary>Returns the identifier of the principal.</summary>
    /// <param name="principalData">The principal.</param>
    TypedSecurityIdentity Extract(PrincipalData principalData);
}