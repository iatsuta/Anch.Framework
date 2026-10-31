namespace Anch.SecuritySystem.AvailableSecurity;

/// <summary>Lists the roles that the current user has.</summary>
public interface IAvailableSecurityRoleSource
{
    /// <summary>Returns the available roles.</summary>
    /// <param name="expandChildren">Whether to include the child roles of the roles the user has.</param>
    IAsyncEnumerable<FullSecurityRole> GetAvailableSecurityRoles(bool expandChildren = true);
}