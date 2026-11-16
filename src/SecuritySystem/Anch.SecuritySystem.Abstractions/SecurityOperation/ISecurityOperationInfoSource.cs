// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>The registry of configured operations.</summary>
public interface ISecurityOperationInfoSource
{
    /// <summary>Returns the registration data of an operation.</summary>
    /// <param name="securityOperation">The operation.</param>
    SecurityOperationInfo GetSecurityOperationInfo(SecurityOperation securityOperation);
}
