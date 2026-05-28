using Xunit.Internal;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestCaseRunner(IServiceProviderPool? serviceProviderPool) : XunitTestCaseRunner
{
    protected override ValueTask<RunSummary> RunTest(XunitTestCaseRunnerContext ctxt, IXunitTest test)
    {
        Guard.ArgumentNotNull(ctxt);
        Guard.ArgumentNotNull(test);

        return new AnchTestRunner(serviceProviderPool).Run(
            test,
            ctxt.MessageBus,
            ctxt.ConstructorArguments,
            ctxt.ExplicitOption,
            ctxt.Aggregator.Clone(),
            ctxt.CancellationTokenSource,
            ctxt.BeforeAfterTestAttributes);
    }
}