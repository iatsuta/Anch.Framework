namespace Anch.OData.DependencyInjection;

public interface IODataSetup
{
    IODataSetup SetParsingExceptionFactory<TParsingExceptionFactory>()
        where TParsingExceptionFactory : IParsingExceptionFactory;
}