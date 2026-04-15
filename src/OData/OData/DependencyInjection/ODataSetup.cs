using CommonFramework.Caching.DependencyInjection;
using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace OData.DependencyInjection;

public class ODataSetup : IODataSetup, IServiceInitializer
{
    private Type? parsingExceptionFactoryType;

    public void Initialize(IServiceCollection services)
    {
        if (!services.AlreadyInitialized<ISelectOperationParser>())
        {
            services
                .AddCommonCaching()
                .AddSingleton<IParsingExceptionFactory, ParsingExceptionFactory>()
                .AddSingleton<ILambdaExpressionConverter, LambdaExpressionConverter>()
                .AddSingleton<ISelectOperationConverter, SelectOperationConverter>()
                .AddSingleton<IRawSelectOperationParser, RawSelectOperationParser>()
                .AddSingleton<ISelectOperationParser, SelectOperationParser>();
        }

        if (this.parsingExceptionFactoryType != null)
        {
            services.ReplaceSingleton(typeof(IParsingExceptionFactory), this.parsingExceptionFactoryType);
        }
    }

    public IODataSetup SetParsingExceptionFactory<TParsingExceptionFactory>()
        where TParsingExceptionFactory : IParsingExceptionFactory
    {
        this.parsingExceptionFactoryType = typeof(TParsingExceptionFactory);

        return this;
    }
}