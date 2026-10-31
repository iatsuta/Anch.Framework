namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>Reads principals across all permission sources.</summary>
public interface IRootPrincipalSourceService : IPrincipalSourceServiceBase
{
    /// <summary>Returns the source service for one principal entity type.</summary>
    /// <param name="principalType">The principal entity type.</param>
    IPrincipalSourceService ForPrincipal(Type principalType);
}