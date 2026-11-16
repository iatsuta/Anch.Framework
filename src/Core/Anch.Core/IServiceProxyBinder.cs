namespace Anch.Core;

/// <summary>Computes the target implementation type for a redirected service, for example by looking at the domain type.</summary>
public interface IServiceProxyBinder
{
    /// <summary>Returns the implementation type to create.</summary>
    Type GetTargetServiceType();
}