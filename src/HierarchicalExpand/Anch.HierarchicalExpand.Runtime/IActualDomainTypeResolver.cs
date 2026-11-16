namespace Anch.HierarchicalExpand;

/// <summary>Maps a runtime type (for example an ORM proxy subclass) to the actual domain type that was registered.</summary>
public interface IActualDomainTypeResolver
{
    /// <summary>Returns the actual domain type.</summary>
    /// <param name="domainType">The runtime type.</param>
    Type Resolve(Type domainType);
}