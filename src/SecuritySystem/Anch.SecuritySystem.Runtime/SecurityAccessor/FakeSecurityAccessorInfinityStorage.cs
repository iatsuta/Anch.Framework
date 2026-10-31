namespace Anch.SecuritySystem.SecurityAccessor;

/// <summary>A storage that reports no unrestricted accessors.</summary>
public class FakeSecurityAccessorInfinityStorage : ISecurityAccessorInfinityStorage
{
    public IEnumerable<string> GetInfinityData()
    {
        throw new InvalidOperationException("Use 'SetSecurityAccessorInfinityStorage' for initialize infinity storage");
    }
}