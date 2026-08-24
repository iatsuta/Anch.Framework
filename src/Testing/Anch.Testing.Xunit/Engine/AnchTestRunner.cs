using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestRunner(IServiceProviderPool? serviceProviderPool) : XunitTestRunnerBase<AnchTestRunnerContext, IXunitTest>
{
    public async ValueTask<RunSummary> Run(
        IXunitTest test,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExplicitOption explicitOption,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        ParallelMode parallelMode,
        ExecutionScheduler scheduler,
        IReadOnlyCollection<IBeforeAfterTestAttribute> beforeAfterAttributes,
        FixtureMappingManager caseFixtureMappings)
    {
        await using var serviceProviderPoolScope = await serviceProviderPool.TryCreateScopeAsync(cancellationTokenSource.Token);

        if (serviceProviderPoolScope?.Exception is null)
        {
            await using var ctxt = new AnchTestRunnerContext(
                test,
                explicitOption,
                messageBus,
                aggregator,
                cancellationTokenSource,
                parallelMode,
                scheduler,
                beforeAfterAttributes,
                constructorArguments.Select(arg => arg == HandledServiceProvider.Instance ? serviceProviderPoolScope?.ServiceProvider : arg).ToArray(),
                caseFixtureMappings
            );

            await ctxt.InitializeAsync();

            return await this.Run(ctxt);
        }
        else
        {
            return XunitRunnerHelper.FailTest(messageBus, cancellationTokenSource, test, serviceProviderPoolScope.Exception);
        }
    }
}
