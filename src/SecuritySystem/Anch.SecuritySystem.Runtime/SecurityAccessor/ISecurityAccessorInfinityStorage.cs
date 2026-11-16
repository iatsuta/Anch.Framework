namespace Anch.SecuritySystem.SecurityAccessor;

/// <summary>Supplies the users that have unrestricted access.</summary>
public interface ISecurityAccessorInfinityStorage
{
    IEnumerable<string> GetInfinityData();
}