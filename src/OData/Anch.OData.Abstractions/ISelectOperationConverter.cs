using Anch.OData.Domain;

namespace Anch.OData;

/// <summary>The second stage of parsing: binds an untyped operation to a domain type and builds LINQ expressions.</summary>
public interface ISelectOperationConverter
{
    /// <summary>Converts the untyped operation.</summary>
    /// <param name="rawSelectOperation">The result of <see cref="IRawSelectOperationParser"/>.</param>
    /// <typeparam name="TDomainObject">The type the query is applied to.</typeparam>
    SelectOperation<TDomainObject> Convert<TDomainObject>(SelectOperation rawSelectOperation);
}