namespace Anch.OData;

/// <summary>The default exception thrown for an invalid OData query string.</summary>
/// <param name="message">The error message.</param>
public class ODataParsingException(string message) : Exception(message);