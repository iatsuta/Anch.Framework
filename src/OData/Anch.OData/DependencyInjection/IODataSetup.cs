namespace Anch.OData.DependencyInjection;

/// <summary>Fluent configuration of the OData module.</summary>
public interface IODataSetup
{
    /// <summary>Replaces the creation of the exception thrown for an invalid query string.</summary>
    /// <typeparam name="TParsingExceptionFactory">The factory type.</typeparam>
    IODataSetup SetParsingExceptionFactory<TParsingExceptionFactory>()
        where TParsingExceptionFactory : IParsingExceptionFactory;
}