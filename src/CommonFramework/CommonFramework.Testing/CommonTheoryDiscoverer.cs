using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonTheoryDiscoverer : TheoryDiscoverer
{
    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod, ITheoryAttribute theoryAttribute)
    {
        if (testMethod is CommonTheoryTestMethod commonTestMethod)
        {
            var serviceProvider = commonTestMethod.ServiceProvider;

            return base.CreateTestCasesForTheory(discoveryOptions, commonTestMethod, theoryAttribute);
        }
        else
        {
            return base.CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
        }
    }

    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForDataRow(ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod, ITheoryAttribute theoryAttribute,
        ITheoryDataRow dataRow, object?[] testMethodArguments)
    {
        return base.CreateTestCasesForDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, testMethodArguments);
    }

    //protected override IXunitTestCase CreateTestCase(
    //    ITestFrameworkDiscoveryOptions discoveryOptions,
    //    IXunitTestMethod testMethod,
    //    IFactAttribute factAttribute)
    //{
    //    if (testMethod is CommonTestMethod commonTestMethod)
    //    {
    //        //ExecutionErrorTestCase
    //        return base.CreateTestCase(discoveryOptions, commonTestMethod, factAttribute);
    //    }
    //    else
    //    {
    //        return base.CreateTestCase(discoveryOptions, testMethod, factAttribute);
    //    }
    //}
}
//var testCase =
//    testMethod.Parameters.Count != 0
//        ? ErrorTestCase(discoveryOptions, testMethod, factAttribute, "[Fact] methods are not allowed to have parameters. Did you mean to use [Theory]?")
//        : testMethod.IsGenericMethodDefinition
//            ? ErrorTestCase(discoveryOptions, testMethod, factAttribute, "[Fact] methods are not allowed to be generic.")
//            : CreateTestCase(discoveryOptions, testMethod, factAttribute);