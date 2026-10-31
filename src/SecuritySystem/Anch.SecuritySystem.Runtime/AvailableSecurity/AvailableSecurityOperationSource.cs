namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Lists the operations included in the roles that the current user has.</summary>
public class AvailableSecurityOperationSource(IAvailableSecurityRoleSource availableSecurityRoleSource)
    : IAvailableSecurityOperationSource
{
    public IAsyncEnumerable<SecurityOperation> GetAvailableSecurityOperations() =>
        availableSecurityRoleSource.GetAvailableSecurityRoles()
            .SelectMany(sr => sr.Information.Operations)
            .Distinct();
}
