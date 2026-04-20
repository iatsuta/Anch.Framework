using System.Data;
using System.Globalization;
using Xunit;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonTheoryDiscoverer : TheoryDiscoverer
{
    public override ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, IXunitTestMethod testMethod, IFactAttribute factAttribute)
    {
        return base.Discover(discoveryOptions, testMethod, factAttribute);
    }

    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForTheory(ITestFrameworkDiscoveryOptions discoveryOptions, IXunitTestMethod testMethod, ITheoryAttribute theoryAttribute)
    {
        if (testMethod.Parameters.LastOrDefault()?.ParameterType != typeof(CancellationToken))
        {
            return base.CreateTestCasesForTheory(discoveryOptions, testMethod, theoryAttribute);
        }

        Guard.ArgumentNotNull(discoveryOptions);
        Guard.ArgumentNotNull(testMethod);
        Guard.ArgumentNotNull(theoryAttribute);

        var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, theoryAttribute);
        var testCase =
            details.SkipReason is not null && details.SkipUnless is null && details.SkipWhen is null
                ? new XunitTestCase(
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
                    timeout: details.Timeout
                )
                : (IXunitTestCase)new XunitDelayEnumeratedTheoryTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    theoryAttribute.SkipTestWithoutData,
                    details.SkipExceptions,
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
                    sourceFilePath: details.SourceFilePath,
                    sourceLineNumber: details.SourceLineNumber,
                    timeout: details.Timeout
                );

#pragma warning disable IDE0300 // Changes the semantics
        return new(new[] { testCase });
#pragma warning restore IDE0300
    }

    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForDataRow(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        ITheoryAttribute theoryAttribute,
        ITheoryDataRow dataRow,
        object?[] testMethodArguments)
    {
        Guard.ArgumentNotNull(discoveryOptions);
        Guard.ArgumentNotNull(testMethod);
        Guard.ArgumentNotNull(theoryAttribute);
        Guard.ArgumentNotNull(dataRow);
        Guard.ArgumentNotNull(testMethodArguments);

        if (testMethod.Parameters.LastOrDefault()?.ParameterType != typeof(CancellationToken))
        {
            return base.CreateTestCasesForDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, testMethodArguments);
        }

        var details = TestIntrospectionHelper.GetTestCaseDetailsForTheoryDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, testMethodArguments);

        if (theoryAttribute is not CulturedTheoryAttribute culturedTheoryAttribute)
            return Error(
                details,
                "{0} was decorated on [{1}] which is not compatible with {2}",
                typeof(CulturedTheoryAttributeDiscoverer).SafeName(),
                theoryAttribute.GetType().SafeName(),
                typeof(CulturedTheoryAttribute).SafeName()
            );

        var cultures = culturedTheoryAttribute.Cultures;
        if (cultures is null || cultures.Length == 0)
            return Error(
                details,
                "{0} did not provide any cultures",
                theoryAttribute.GetType().SafeName()
            );

        var traits = TestIntrospectionHelper.GetTraits(testMethod, dataRow);

        return new(
            cultures
                .Select(
                    culture => new CulturedXunitTestCase(
                        culture,
                        details.ResolvedTestMethod,
                        details.TestCaseDisplayName,
                        details.UniqueID,
                        details.Explicit,
                        details.SkipExceptions,
                        details.SkipReason,
                        details.SkipType,
                        details.SkipUnless,
                        details.SkipWhen,
                        traits,
                        [.. testMethodArguments, CancellationToken.None],
                        timeout: details.Timeout
                    ))
                .CastOrToReadOnlyCollection()
        );


        static ValueTask<IReadOnlyCollection<IXunitTestCase>> Error(
            (string TestCaseDisplayName, bool, Type[]?, string?, Type?, string?, string?, string? SourceFilePath, int? SourceLineNumber, int, string UniqueID, IXunitTestMethod ResolvedTestMethod) details,
            string format,
            params object?[] args) =>
            new([
#pragma warning disable CA2000 // The disposable object is returned via the ValueTask
                new ExecutionErrorTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    string.Format(CultureInfo.CurrentCulture, format, args)
                )
#pragma warning restore CA2000
            ]);
    }
}