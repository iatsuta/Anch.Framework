namespace SecuritySystem.Testing;

public interface ITestingEvaluator<out TService>
{
    Task<TResult> EvaluateAsync<TResult>(TestingScopeMode mode, UserCredential? userCredential, Func<TService, Task<TResult>> evaluate);
}