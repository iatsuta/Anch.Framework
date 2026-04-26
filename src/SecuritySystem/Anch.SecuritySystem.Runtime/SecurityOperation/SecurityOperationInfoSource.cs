using System.Collections.Frozen;

using Anch.Core;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public class SecurityOperationInfoSource(IEnumerable<FullSecurityOperation> securityOperations) : ISecurityOperationInfoSource
{
    private readonly FrozenDictionary<SecurityOperation, SecurityOperationInfo> dict = securityOperations.ToFrozenDictionary(
        pair => pair.SecurityOperation,
        pair => pair.Info);

    public SecurityOperationInfo GetSecurityOperationInfo(SecurityOperation securityOperation) =>
        this.dict.GetValueOrDefault(securityOperation, () => new SecurityOperationInfo());
}
