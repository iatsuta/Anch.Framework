using Anch.OData.Domain;

namespace Anch.OData;

/// <summary>The first stage of parsing: turns the query string into an untyped syntax tree that does not yet know the domain type.</summary>
public interface IRawSelectOperationParser
{
    /// <summary>Parses the query string.</summary>
    /// <param name="input">The OData query string.</param>
    SelectOperation Parse(string input);
}