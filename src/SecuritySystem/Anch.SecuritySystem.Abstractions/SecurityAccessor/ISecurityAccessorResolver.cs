namespace Anch.SecuritySystem.SecurityAccessor;

public interface ISecurityAccessorResolver
{
    public const string RawKey = "Raw";

    IEnumerable<string> Resolve(SecurityAccessorData data);
}
