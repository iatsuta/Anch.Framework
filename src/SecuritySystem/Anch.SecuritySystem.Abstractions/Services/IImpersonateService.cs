namespace Anch.SecuritySystem.Services;

/// <summary>Runs code on behalf of another user for the duration of a call.</summary>
public interface IImpersonateService
{
    /// <summary>Runs <paramref name="func"/> while the given user is the current user.</summary>
    /// <param name="customUserCredential">The user to act as.</param>
    /// <param name="func">The code to run.</param>
    /// <typeparam name="T">The result type.</typeparam>
    Task<T> WithImpersonateAsync<T>(UserCredential customUserCredential, Func<Task<T>> func);
}