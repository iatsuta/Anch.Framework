namespace Anch.OData;

/// <summary>The default factory: creates an <see cref="ODataParsingException"/> that names the unparsed part of the query.</summary>
public class ParsingExceptionFactory : IParsingExceptionFactory
{
    /// <inheritdoc/>
    public virtual Exception GetError(string input, string unparsedRest)
    {
        return new ODataParsingException($"Failed to parse input. Unparsed part: '{unparsedRest}'");
    }
}