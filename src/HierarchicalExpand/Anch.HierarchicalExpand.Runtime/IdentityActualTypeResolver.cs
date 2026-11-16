namespace Anch.HierarchicalExpand;

/// <summary>The default resolver: returns the type unchanged.</summary>
public class IdentityActualTypeResolver : IActualDomainTypeResolver
{
    /// <inheritdoc/>
    public Type Resolve(Type domainType)
    {
        return domainType;
    }
}