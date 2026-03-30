using Microsoft.Extensions.DependencyInjection;

namespace OData.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOData(this IServiceCollection services)
    {
        return services
            .AddSingleton<ILambdaExpressionConverter, LambdaExpressionConverter>()
            .AddSingleton<IRawSelectOperationParser, RawSelectOperationParser>()
            .AddSingleton<ISelectOperationParser, SelectOperationParser>();
    }
}