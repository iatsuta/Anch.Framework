using System.Reflection;

using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit.Engine;

public class AnchTheoryTestMethod(IXunitTestMethod baseMethod) : IXunitTestMethod, IXunitSerializable, IServiceProviderPoolContainer
{
    private IReadOnlyCollection<IDataAttribute>? dataAttributes;

    public AnchTheoryTestMethod() : this(new XunitTestMethod())
    {
    }

    public IServiceProviderPool? ServiceProviderPool
    {
        get => field;
        set
        {
            if (field != value)
            {
                this.dataAttributes = null;
                field = value;
            }
        }
    }

    public int? MethodArity => baseMethod.MethodArity;

    public string MethodName => baseMethod.MethodName;

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> Traits => baseMethod.Traits;

    public string UniqueID => baseMethod.UniqueID;

    public IReadOnlyCollection<IBeforeAfterTestAttribute> BeforeAfterTestAttributes => baseMethod.BeforeAfterTestAttributes;

    public IReadOnlyCollection<IDataAttribute> DataAttributes => this.dataAttributes ??=
    [
        .. baseMethod.DataAttributes.Select(attr =>
        {
            if (attr is IServiceProviderPoolContainer serviceProviderPoolContainer)
            {
                serviceProviderPoolContainer.ServiceProviderPool = this.ServiceProviderPool;
            }

            return attr;
        })
    ];

    public IReadOnlyCollection<IFactAttribute> FactAttributes => baseMethod.FactAttributes;

    public bool IsGenericMethodDefinition => baseMethod.IsGenericMethodDefinition;

    public MethodInfo Method => baseMethod.Method;

    public IReadOnlyCollection<ParameterInfo> Parameters => baseMethod.Parameters;

    public Type ReturnType => baseMethod.ReturnType;

    public object?[] TestMethodArguments => baseMethod.TestMethodArguments;

    public IXunitTestClass TestClass => baseMethod.TestClass;

    ITestClass ITestMethod.TestClass => this.TestClass;

    public string GetDisplayName(
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

    public MethodInfo MakeGenericMethod(Type[] genericTypes) => baseMethod.MakeGenericMethod(genericTypes);

    public Type[]? ResolveGenericTypes(object?[] arguments) => baseMethod.ResolveGenericTypes(arguments);

    public object?[] ResolveMethodArguments(object?[] arguments) => baseMethod.ResolveMethodArguments(arguments);

    public void Deserialize(IXunitSerializationInfo info) => (baseMethod as IXunitSerializable)?.Deserialize(info);

    public void Serialize(IXunitSerializationInfo info) => (baseMethod as IXunitSerializable)?.Serialize(info);
}