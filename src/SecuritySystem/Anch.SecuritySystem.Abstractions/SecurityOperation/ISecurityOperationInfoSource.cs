// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public interface ISecurityOperationInfoSource
{
    SecurityOperationInfo GetSecurityOperationInfo(SecurityOperation securityOperation);
}
