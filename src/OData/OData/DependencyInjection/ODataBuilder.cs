using CommonFramework.Caching.DependencyInjection;
using CommonFramework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace OData.DependencyInjection;

public class ODataBuilder : IODataBuilder, IServiceInitializer
{
    public void Initialize(IServiceCollection services)
    {
        if (!services.AlreadyInitialized<ISelectOperationParser>())
        {
            services
                .AddCommonCaching()
                .AddSingleton<ILambdaExpressionConverter, LambdaExpressionConverter>()
                .AddSingleton<ISelectOperationConverter, SelectOperationConverter>()
                .AddSingleton<IRawSelectOperationParser, RawSelectOperationParser>()
                .AddSingleton<ISelectOperationParser, SelectOperationParser>();
        }
    }
}