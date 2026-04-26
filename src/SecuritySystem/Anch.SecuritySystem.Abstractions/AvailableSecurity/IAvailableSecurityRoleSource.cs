namespace Anch.SecuritySystem.AvailableSecurity;

public interface IAvailableSecurityRoleSource
{
    IAsyncEnumerable<FullSecurityRole> GetAvailableSecurityRoles(bool expandChildren = true);
}