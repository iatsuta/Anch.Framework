using System.Reflection;

using Xunit.v3;

namespace CommonFramework.Testing;

public class CommonTestClassRunner(IServiceProvider rootServiceProvider) : XunitTestClassRunner
{
    protected override ValueTask<object?> GetConstructorArgument(
        XunitTestClassRunnerContext ctxt,
        ConstructorInfo constructor,
        int index,
        ParameterInfo parameter)
    {
        if (parameter.ParameterType == typeof(IServiceProvider))
        {
            return new ValueTask<object?>(rootServiceProvider);
        }
        else
        {
            return base.GetConstructorArgument(ctxt, constructor, index, parameter);
        }
    }
}