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
}