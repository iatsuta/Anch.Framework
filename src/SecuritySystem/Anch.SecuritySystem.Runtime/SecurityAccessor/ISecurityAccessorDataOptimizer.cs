namespace Anch.SecuritySystem.SecurityAccessor;

/// <summary>Simplifies a description of who has access.</summary>
public interface ISecurityAccessorDataOptimizer
{
    public SecurityAccessorData Optimize(SecurityAccessorData result);
}
