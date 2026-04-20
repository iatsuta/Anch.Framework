using Xunit.v3;

namespace CommonFramework.Testing.Engine;

public class CommonXunitTestCase(
    IXunitTestMethod testMethod,
    string testCaseDisplayName,
    string uniqueId,
    bool @explicit,
    Type[]? skipExceptions = null,
    string? skipReason = null,
    Type? skipType = null,
    string? skipUnless = null,
    string? skipWhen = null,
    Dictionary<string, HashSet<string>>? traits = null,
    object?[]? testMethodArguments = null,
    string? sourceFilePath = null,
    int? sourceLineNumber = null,
    int? timeout = null)
    : XunitTestCase(testMethod, testCaseDisplayName, uniqueId, @explicit, skipExceptions, skipReason, skipType, skipUnless, skipWhen, traits,
        testMethodArguments,
        sourceFilePath, sourceLineNumber, timeout);