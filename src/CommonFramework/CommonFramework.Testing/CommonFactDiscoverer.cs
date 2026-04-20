using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonFactDiscoverer : FactDiscoverer
{
    public override ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        IFactAttribute factAttribute)
    {
        if (testMethod.Parameters.Count == 1 && testMethod.Parameters.Single().ParameterType == typeof(CancellationToken))
        {
            return new([this.CreateTestCase(discoveryOptions, testMethod, factAttribute)]);
        }
        else
        {
            return base.Discover(discoveryOptions, testMethod, factAttribute);
        }
    }

    protected override IXunitTestCase CreateTestCase(ITestFrameworkDiscoveryOptions discoveryOptions, IXunitTestMethod testMethod, IFactAttribute factAttribute)
    {
        if (testMethod.Parameters.LastOrDefault()?.ParameterType == typeof(CancellationToken))
        {
            var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, factAttribute);

            return new CommonXunitTestCase(
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                details.SkipExceptions,
                details.SkipReason,
                details.SkipType,
                details.SkipUnless,
                details.SkipWhen,
                testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
                sourceFilePath: details.SourceFilePath,
                sourceLineNumber: details.SourceLineNumber,
                timeout: details.Timeout,
                testMethodArguments: [CancellationToken.None]
            );
        }
        else
        {
            return base.CreateTestCase(discoveryOptions, testMethod, factAttribute);
        }
    }
}