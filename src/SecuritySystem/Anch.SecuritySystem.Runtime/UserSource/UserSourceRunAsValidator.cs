namespace Anch.SecuritySystem.UserSource;

public class ExistUserRunAsValidator<TUser>(IMissedUserErrorSource missedUserErrorSource, IUserSource<TUser> userSource) : IRunAsValidator
{
    public async ValueTask ValidateAsync(UserCredential value, CancellationToken ct)
    {
        var user = await userSource.TryGetUserAsync(value, ct);

        if (user is null)
        {
            throw missedUserErrorSource.GetNotFoundException(typeof(TUser), value);
        }
    }
}