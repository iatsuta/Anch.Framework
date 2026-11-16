namespace Anch.SecuritySystem.ExternalSystem.Management;

/// <summary>A listener that receives untyped principal and permission data.</summary>
public interface IRawPrincipalManagementListener : IPrincipalManagementListenerBase<PrincipalData, PermissionData>;