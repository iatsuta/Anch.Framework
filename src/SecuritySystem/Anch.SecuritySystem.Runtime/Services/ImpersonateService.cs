namespace Anch.SecuritySystem.Services;

/// <summary>Runs code on behalf of another user by setting the impersonation state for the duration of the call.</summary>
public class ImpersonateService(ImpersonateState impersonateState) : IImpersonateService
{
    public async Task<T> WithImpersonateAsync<T>(UserCredential customUserCredential, Func<Task<T>> func)
    {
        var prev = impersonateState.CustomUserCredential;

        impersonateState.CustomUserCredential = customUserCredential;

        try
        {
            return await func();
        }
        finally
        {
            impersonateState.CustomUserCredential = prev;
        }
    }
}