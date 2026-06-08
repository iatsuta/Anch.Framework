using Anch.Core;
using Anch.GenericQueryable;

namespace Anch.SecuritySystem.UserSource;

public class UserSource<TUser>(IUserQueryableSource<TUser> userQueryableSource, IMissedUserService<TUser> missedUserService) : IUserSource<TUser>
    where TUser : class
{
    private readonly Dictionary<UserCredential, TUser?> tryGetUserCache = new();

    private readonly Dictionary<UserCredential, TUser> getUserCache = new();

    private IUserSource<User>? simpleCache;

    public Type UserType { get; } = typeof(TUser);

    public Task<TUser?> TryGetUserAsync(UserCredential userCredential, CancellationToken ct)
    {
        return this.tryGetUserCache.GetValueOrCreateAsync(userCredential,
            () => userQueryableSource.GetQueryable(userCredential).GenericSingleOrDefaultAsync(ct));
    }

    public Task<TUser> GetUserAsync(UserCredential userCredential, CancellationToken ct)
    {
        return this.getUserCache.GetValueOrCreateAsync(userCredential, async () =>
            await this.TryGetUserAsync(userCredential, ct) ?? missedUserService.GetUser(userCredential));
    }

    public IUserSource<User> ToSimple()
    {
        return this.simpleCache ??= new UserSource<User>(userQueryableSource.ToSimple(), missedUserService.ToSimple());
    }
}