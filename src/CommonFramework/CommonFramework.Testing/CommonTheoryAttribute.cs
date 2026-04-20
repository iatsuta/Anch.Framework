using Xunit.v3;

namespace CommonFramework.Testing;

[XunitTestCaseDiscoverer(typeof(CommonTheoryDiscoverer))]
public class CommonTheoryAttribute(ITheoryAttribute baseAttribute) : ITheoryAttribute
{
    public string? DisplayName => baseAttribute.DisplayName;

    public bool Explicit => baseAttribute.Explicit;

    public string? Skip => baseAttribute.Skip;

    public Type[]? SkipExceptions => baseAttribute.SkipExceptions;

    public Type? SkipType => baseAttribute.SkipType;

    public string? SkipUnless => baseAttribute.SkipUnless;

    public string? SkipWhen => baseAttribute.SkipWhen;

    public string? SourceFilePath => baseAttribute.SourceFilePath;

    public int? SourceLineNumber => baseAttribute.SourceLineNumber;

    public int Timeout => baseAttribute.Timeout;

    public bool DisableDiscoveryEnumeration => baseAttribute.DisableDiscoveryEnumeration;

    public bool SkipTestWithoutData => baseAttribute.SkipTestWithoutData;
}