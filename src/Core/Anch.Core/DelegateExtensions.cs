namespace Anch.Core;

public static class DelegateExtensions
{
    public static System.Reflection.MethodInfo CreateGenericMethod(this Delegate source, params Type[] types)
    {
        return source.Method.GetGenericMethodDefinition().MakeGenericMethod(types);
    }
}