namespace Anch.OData;

/// <summary>Creates the exception thrown for an invalid query string. Replace it to throw an application-specific exception (for example one that maps to HTTP 400).</summary>
public interface IParsingExceptionFactory
{
    /// <summary>Creates the exception.</summary>
    /// <param name="input">The whole query string.</param>
    /// <param name="unparsedRest">The part of the string that could not be parsed.</param>
    Exception GetError(string input, string unparsedRest);
}