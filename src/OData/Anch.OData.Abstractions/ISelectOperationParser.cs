using Anch.OData.Domain;

namespace Anch.OData;

/// <summary>
/// The main entry point of the OData module: parses an OData query string (<c>$filter</c>, <c>$orderby</c>, <c>$top</c>, <c>$skip</c>)
/// into a typed <see cref="SelectOperation{TDomainObject}"/> that can be applied to an <see cref="IQueryable{T}"/>. Results are cached by query string.
/// </summary>
/// <example>
/// <code>
/// var operation = parser.Parse&lt;Employee&gt;("$filter=Active eq true&amp;$top=30");
/// var query = operation.Inject(db.Employees);
/// </code>
/// </example>
public interface ISelectOperationParser
{
    /// <summary>Parses the query string and binds it to the properties of the domain type.</summary>
    /// <param name="input">The OData query string.</param>
    /// <typeparam name="TDomainObject">The type the query is applied to.</typeparam>
    /// <exception cref="ODataParsingException">The string is not a valid query (or the exception type configured with <c>SetParsingExceptionFactory</c>).</exception>
    SelectOperation<TDomainObject> Parse<TDomainObject>(string input);
}