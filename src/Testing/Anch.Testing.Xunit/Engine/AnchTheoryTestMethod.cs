using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTheoryTestMethod : XunitTestMethod, IXunitTestMethod
{
    private readonly IXunitTestMethod baseMethod;

    private readonly IServiceProviderPool? serviceProviderPool;

    public AnchTheoryTestMethod()
    {
    }

    public AnchTheoryTestMethod(IXunitTestMethod baseMethod, IServiceProviderPool? serviceProviderPool)
        : base(baseMethod.TestClass, baseMethod.Method, baseMethod.TestMethodArguments, baseMethod.UniqueID)
    {
        this.baseMethod = baseMethod;
        this.serviceProviderPool = serviceProviderPool;
    }

    IReadOnlyCollection<IDataAttribute> IXunitTestMethod.DataAttributes => field ??=
    [
        .. baseMethod.DataAttributes.Select(attr =>
        {
            if (attr is IServiceProviderPoolAttribute serviceProviderPoolAttribute)
            {
                serviceProviderPoolAttribute.ServiceProviderPool = serviceProviderPool;
            }

            return attr;
        })
    ];

    string IXunitTestMethod.GetDisplayName(
        string baseDisplayName,
        string? label,
        object?[]? testMethodArguments,
        Type[]? methodGenericTypes)
    {
        var displayName = baseMethod.GetDisplayName(baseDisplayName, label, testMethodArguments, methodGenericTypes);

        if (testMethodArguments != null && baseMethod.Method.LastParameterIsCt() && baseMethod.Method.LastParameterIsCt() && displayName.EndsWith("???)"))
        {
            var skipPattern = $", {baseMethod.Method.GetParameters().Last().Name}: ???)";

            if (displayName.EndsWith(skipPattern))
            {
                return displayName[..^skipPattern.Length] + ")";
            }
        }

        return displayName;
    }
}