namespace Anch.SecuritySystem.Services;

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