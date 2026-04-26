namespace Anch.OData;

public class ParsingExceptionFactory : IParsingExceptionFactory
{
    public virtual Exception GetError(string input, string unparsedRest)
    {
        return new ODataParsingException($"Failed to parse input. Unparsed part: '{unparsedRest}'");
    }
}