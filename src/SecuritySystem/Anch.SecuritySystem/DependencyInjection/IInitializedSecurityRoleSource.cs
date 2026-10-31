namespace Anch.SecuritySystem.DependencyInjection;

/// <summary>The roles registered before initialization, with their final registration data.</summary>
public interface IInitializedSecurityRoleSource
{
    IEnumerable<FullSecurityRole> GetSecurityRoles();
}
