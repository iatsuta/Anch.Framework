using System.Collections;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Runtime.ExceptionServices;

using Anch.Testing.Xunit.Engine;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace Anch.Testing.Xunit;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class AnchMemberDataAttribute : MemberDataAttributeBase, IServiceProviderPoolContainer
{
    private IServiceProviderPool? serviceProviderPool;

    private readonly ConcurrentDictionary<MethodInfo, Task<IReadOnlyCollection<ITheoryDataRow>>> testDataCache = [];

    private static readonly Func<Lazy<string>> GetSupportedDataSignatures = PrivateMemberAccessor.GetStaticField<Lazy<string>>(typeof(MemberDataAttributeBase), "supportedDataSignatures");


    public AnchMemberDataAttribute(string memberName, params object?[] arguments)
        : base(memberName, arguments)
    {
        this.DisableDiscoveryEnumeration = true;
    }

    private object? TryCreateTestInstance(MethodInfo testMethod, IServiceProvider? serviceProvider)
    {
        if (testMethod.IsStatic)
        {
            return null;
        }
        else
        {
            var testType = testMethod.ReflectedType!;

            if (serviceProvider == null)
            {
                return Activator.CreateInstance(testType);
            }
            else
            {
                return ActivatorUtilities.CreateInstance(serviceProvider, testType);
            }
        }
    }

    /// <inheritdoc/>
    public override async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker _) =>

        await this.testDataCache.GetOrAdd(testMethod, async _ =>
        {
            if (this.MemberType is null)
                return [];

            var accessor = this.GetPropertyAccessor(this.MemberType)
                           ?? this.GetFieldAccessor(this.MemberType)
                           ?? this.GetMethodAccessor(this.MemberType)
                           ?? throw new ArgumentException(
                               string.Format(
                                   CultureInfo.CurrentCulture,
                                   "Could not find public static member (property, field, or method) named '{0}' on '{1}'{2}",
                                   this.MemberName, this.MemberType.SafeName(), this.Arguments.Length > 0
                                       ? string.Format(CultureInfo.CurrentCulture, " with parameter types: {0}",
                                           string.Join(", ",
                                               this.Arguments.Select(p => p?.GetType().SafeName() ?? "(null)")))
                                       : ""
                               )
                           );

            var ct = TestContext.Current.CancellationToken;

            await using var serviceProviderPoolScope = await this.serviceProviderPool.TryCreateScopeAsync(ct);

            if (serviceProviderPoolScope?.Exception != null)
            {
                ExceptionDispatchInfo.Capture(serviceProviderPoolScope.Exception).Throw();
            }

            var testInstance = this.TryCreateTestInstance(testMethod, serviceProviderPoolScope?.ServiceProvider);

            await using var __ = await (testInstance as IAsyncLifetime).TryCreateScopeAsync(ct);

            var returnValue =
                accessor(testInstance)
                ?? throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Member '{0}' on '{1}' returned null when queried for test data", this.MemberName,
                        this.MemberType.SafeName()
                    ));

            if (returnValue is IEnumerable dataItems)
            {
                var result = new List<ITheoryDataRow>();

                foreach (var dataItem in dataItems)
                    if (dataItem is not null)
                        result.Add(this.ConvertDataRow(dataItem));

                return result.CastOrToReadOnlyCollection();
            }

            return await this.GetDataAsync(returnValue, this.MemberType);
        });

    async ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetDataAsync(
        object? returnValue,
        Type type)
    {
        var taskAwaitable = returnValue.AsValueTask();
        if (taskAwaitable.HasValue)
            returnValue = await taskAwaitable.Value;

        if (returnValue is IAsyncEnumerable<object?> asyncDataItems)
        {
            var result = new List<ITheoryDataRow>();

            await foreach (var dataItem in asyncDataItems)
                if (dataItem is not null)
                    result.Add(this.ConvertDataRow(dataItem));

            return result.CastOrToReadOnlyCollection();
        }

        // Duplicate from GetData(), but it's hard to avoid since we need to support Task/ValueTask
        // of IEnumerable (and not just IAsyncEnumerable).
        if (returnValue is IEnumerable dataItems)
        {
            var result = new List<ITheoryDataRow>();

            foreach (var dataItem in dataItems)
                if (dataItem is not null)
                    result.Add(this.ConvertDataRow(dataItem));

            return result.CastOrToReadOnlyCollection();
        }

        throw new ArgumentException(
            string.Format(
                CultureInfo.CurrentCulture,
                "Member '{0}' on '{1}' must return data in one of the following formats:{2}{3}", this.MemberName,
                type.SafeName(),
                Environment.NewLine,
                GetSupportedDataSignatures().Value
            )
        );
    }

    Func<object?, object?>? GetFieldAccessor(Type? type)
    {
        FieldInfo? fieldInfo = null;
        foreach (var reflectionType in GetTypesForMemberResolution(type, includeInterfaces: false))
        {
            fieldInfo = reflectionType.GetRuntimeField(this.MemberName);
            if (fieldInfo is not null)
                break;
        }

        return
            fieldInfo is not null
                ? instance => fieldInfo.GetValue(instance)
                : null;
    }

    Func<object?, object?>? GetMethodAccessor(Type? type)
    {
        MethodInfo? methodInfo = null;
        var argumentTypes = this.Arguments is null ? [] : this.Arguments.Select(p => p?.GetType()).ToArray();
        foreach (var reflectionType in GetTypesForMemberResolution(type, includeInterfaces: true))
        {
            var methodInfoArray =
                reflectionType
                    .GetRuntimeMethods()
                    .Where(m => m.Name == this.MemberName && ParameterTypesCompatible(m.GetParameters(), argumentTypes))
                    .ToArray();
            if (methodInfoArray.Length == 0)
                continue;
            if (methodInfoArray.Length == 1)
            {
                methodInfo = methodInfoArray[0];
                break;
            }

            methodInfo = methodInfoArray.Where(m => m.GetParameters().Length == argumentTypes.Length).FirstOrDefault();
            if (methodInfo is not null)
                break;

            throw new ArgumentException(
                string.Format(
                    CultureInfo.CurrentCulture,
                    "The call to method '{0}.{1}' is ambigous between {2} different options for the given arguments.",
                    type!.SafeName(), this.MemberName,
                    methodInfoArray.Length
                ),
                nameof(type)
            );
        }

        if (methodInfo is null)
            return null;

        var completedArguments = this.Arguments ?? [];
        var finalMethodParameters = methodInfo.GetParameters();

        completedArguments =
            completedArguments.Length == finalMethodParameters.Length
                ? completedArguments
                : completedArguments.Concat(finalMethodParameters.Skip(completedArguments.Length).Select(pi => pi.DefaultValue)).ToArray();

        return instance => methodInfo.Invoke(instance, completedArguments);
    }

    Func<object?, object?>? GetPropertyAccessor(Type? type)
    {
        PropertyInfo? propInfo = null;
        foreach (var reflectionType in GetTypesForMemberResolution(type, includeInterfaces: true))
        {
            propInfo = reflectionType.GetRuntimeProperty(this.MemberName);
            if (propInfo is not null)
                break;
        }

        return
            propInfo is not null && propInfo.GetMethod is not null
                ? instance => propInfo.GetValue(instance, null)
                : null;
    }

    static IEnumerable<Type> GetTypesForMemberResolution(
        Type? typeToInspect,
        bool includeInterfaces)
    {
        HashSet<Type> interfaces = [];

        for (var reflectionType = typeToInspect; reflectionType is not null; reflectionType = reflectionType.BaseType)
        {
            yield return reflectionType;

            if (includeInterfaces)
                foreach (var @interface in reflectionType.GetInterfaces())
                    interfaces.Add(@interface);
        }

        foreach (var @interface in interfaces)
            yield return @interface;
    }

    static bool ParameterTypesCompatible(
        ParameterInfo[] parameters,
        Type?[] argumentTypes)
    {
        if (parameters.Length < argumentTypes.Length)
            return false;

        var idx = 0;
        for (; idx < argumentTypes.Length; ++idx)
            if (argumentTypes[idx] is not null && !parameters[idx].ParameterType.IsAssignableFrom(argumentTypes[idx]!))
                return false;

        for (; idx < parameters.Length; ++idx)
            if (!parameters[idx].IsOptional)
                return false;

        return true;
    }

    /// <inheritdoc/>
    public override bool SupportsDiscoveryEnumeration() =>
        !this.DisableDiscoveryEnumeration;

    IServiceProviderPool? IServiceProviderPoolContainer.ServiceProviderPool
    {
        get => this.serviceProviderPool;
        set => this.serviceProviderPool = value;
    }
}