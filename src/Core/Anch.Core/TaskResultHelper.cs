namespace Anch.Core;

/// <summary>Helpers for working with the result of a task of a given type.</summary>
public static class TaskResultHelper<TResult>
{
    public static readonly bool IsTask = typeof(Task).IsAssignableFrom(typeof(TResult));

    public static void TypeIsNotTaskValidate(string errorMessage = "For Task result use EvaluateAsync method")
    {
        if (IsTask)
        {
            throw new Exception(errorMessage);
        }
    }
}