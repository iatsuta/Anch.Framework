using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTestCase(IXunitTestCase baseTestCase, IXunitTestMethod testMethod) : IXunitTestCase
{
    public AnchTestCase()
        : this(new XunitTestCase(), new XunitTestMethod())
    {
    }

    public bool Explicit => baseTestCase.Explicit;

    public bool DisableParallelization => baseTestCase.DisableParallelization;

    string? ICoreTestCase.SkipReason => baseTestCase.SkipReason;

    public Type? SkipType => baseTestCase.SkipType;
    public string? SkipUnless => baseTestCase.SkipUnless;
    public string? SkipWhen => baseTestCase.SkipWhen;

    public IXunitTestClass TestClass => baseTestCase.TestClass;

    ICoreTestClass ICoreTestCase.TestClass => baseTestCase.TestClass;

    public IXunitTestCollection TestCollection => baseTestCase.TestCollection;

    ICoreTestCollection ICoreTestCase.TestCollection => baseTestCase.TestCollection;

    public IXunitTestMethod TestMethod => testMethod;

    ICoreTestMethod ICoreTestCase.TestMethod => testMethod;

    public int TestMethodMetadataToken => baseTestCase.TestMethodMetadataToken;

    string ICoreTestCase.TestMethodName => baseTestCase.TestMethodName;

    string[] IXunitTestCase.TestMethodParameterTypesVSTest => baseTestCase.TestMethodParameterTypesVSTest;

    string IXunitTestCase.TestMethodReturnTypeVSTest => baseTestCase.TestMethodReturnTypeVSTest;

    public int Timeout => baseTestCase.Timeout;

    public int TestClassMetadataToken => baseTestCase.TestClass.MetadataToken;

    string ICoreTestCase.TestClassName => baseTestCase.TestClassName;

    string ICoreTestCase.TestClassSimpleName => baseTestCase.TestClassSimpleName;

    int ICoreTestCase.TestMethodArity => baseTestCase.TestMethodArity;

    public ValueTask<IReadOnlyCollection<IXunitTest>> CreateTests() => baseTestCase.CreateTests();

    public void PostInvoke() => baseTestCase.PostInvoke();

    public void PreInvoke() => baseTestCase.PreInvoke();

    public Type[]? SkipExceptions => baseTestCase.SkipExceptions;

    string? ITestCaseMetadata.SkipReason => baseTestCase.SkipReason;

    public string? SourceFilePath => baseTestCase.SourceFilePath;

    public int? SourceLineNumber => baseTestCase.SourceLineNumber;

    public string TestCaseDisplayName => baseTestCase.TestCaseDisplayName;

    int? ITestCaseMetadata.TestClassMetadataToken => baseTestCase.TestClassMetadataToken;

    string? ITestCaseMetadata.TestClassName => baseTestCase.TestClassName;

    public string? TestClassNamespace => baseTestCase.TestClassNamespace;

    string? ITestCaseMetadata.TestClassSimpleName => baseTestCase.TestClassSimpleName;

    int? ITestCaseMetadata.TestMethodArity => baseTestCase.TestMethodArity;

    int? ITestCaseMetadata.TestMethodMetadataToken => baseTestCase.TestMethodMetadataToken;

    string? ITestCaseMetadata.TestMethodName => baseTestCase.TestMethodName;

    string[]? ITestCaseMetadata.TestMethodParameterTypesVSTest => baseTestCase.TestMethodParameterTypesVSTest;

    string? ITestCaseMetadata.TestMethodReturnTypeVSTest => baseTestCase.TestMethodReturnTypeVSTest;

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits => baseTestCase.Traits;

    public string UniqueID => baseTestCase.UniqueID;

    ITestClass? ITestCase.TestClass => baseTestCase.TestClass;

    ITestCollection ITestCase.TestCollection => baseTestCase.TestCollection;

    ITestMethod? ITestCase.TestMethod => baseTestCase.TestMethod;
}
