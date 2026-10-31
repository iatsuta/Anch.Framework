namespace Anch.SecuritySystem.Services;

/// <summary>Converts text identifiers into identifiers of the right type for a domain type.</summary>
public interface IDomainObjectIdentsParser
{
    /// <summary>Parses the identifiers.</summary>
    /// <param name="domainObjectType">The domain type whose identifier type is used.</param>
    /// <param name="idents">The identifiers as text.</param>
    /// <returns>An array of identifiers of the identifier type of the domain type.</returns>
    Array Parse(Type domainObjectType, IEnumerable<string> idents);
}