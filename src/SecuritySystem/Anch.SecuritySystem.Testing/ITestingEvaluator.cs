namespace Anch.SecuritySystem.Testing;

/// <summary>Runs test code in a scope, optionally on behalf of a given user.</summary>
/// <typeparam name="TService">The service the code receives.</typeparam>
public interface ITestingEvaluator<out TService>
{
    /// <summary>Runs the code.</summary>
    /// <param name="mode">Whether the code only reads data or also changes it.</param>
    /// <param name="userCredential">The user to run as; <c>null</c> for the default test user.</param>
    /// <param name="evaluate">The code to run.</param>
    /// <typeparam name="TResult">The result type.</typeparam>
    Task<TResult> EvaluateAsync<TResult>(TestingScopeMode mode, UserCredential? userCredential, Func<TService, Task<TResult>> evaluate);
}