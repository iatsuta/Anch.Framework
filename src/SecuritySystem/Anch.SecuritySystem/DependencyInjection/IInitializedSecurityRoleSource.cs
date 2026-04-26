namespace Anch.SecuritySystem.DependencyInjection;

public interface IInitializedSecurityRoleSource
{
    IEnumerable<FullSecurityRole> GetSecurityRoles();
}
