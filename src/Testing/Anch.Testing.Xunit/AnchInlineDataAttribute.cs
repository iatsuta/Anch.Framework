using System.Reflection;

using Xunit;
using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AnchInlineDataAttribute(params object?[]? data) : Attribute, IDataAttribute
{
    private readonly InlineDataAttribute innerAttr = new(data);

    private IDataAttribute InnerAttrI => this.innerAttr;

    public ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker) =>
        this.InnerAttrI.GetData(testMethod, disposalTracker);

    public bool SupportsDiscoveryEnumeration() => this.InnerAttrI.SupportsDiscoveryEnumeration();

    public string? Label
    {
        get => this.innerAttr.Label;
        set => this.innerAttr.Label = value;
    }

    public string? Skip
    {
        get => this.innerAttr.Skip;
        set => this.innerAttr.Skip = value;
    }

    public Type? SkipType
    {
        get => this.innerAttr.SkipType;
        set => this.innerAttr.SkipType = value;
    }

    public string? SkipUnless
    {
        get => this.innerAttr.SkipUnless;
        set => this.innerAttr.SkipUnless = value;
    }

    public string? SkipWhen
    {
        get => this.innerAttr.SkipWhen;
        set => this.innerAttr.SkipWhen = value;
    }

    public string? TestDisplayName
    {
        get => this.innerAttr.TestDisplayName;
        set => this.innerAttr.TestDisplayName = value;
    }

    public string[]? Traits
    {
        get => this.innerAttr.Traits;
        set => this.innerAttr.Traits = value;
    }

    int? IDataAttribute.Timeout => this.InnerAttrI.Timeout;

    bool? IDataAttribute.Explicit => this.InnerAttrI.Explicit;

    bool IDataAttribute.DisableParallelization => this.InnerAttrI.DisableParallelization;
}