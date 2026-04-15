namespace OData;

public interface IParsingExceptionFactory
{
    Exception GetError(string input, string unparsedRest);
}