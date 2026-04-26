using Anch.Core;
using Anch.Core.Auth;

namespace Anch.SecuritySystem.UserSource;

public class CurrentUserSource<TUser>(
    ICurrentUser currentUser,
    IUserSource<TUser> userSource,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null) : ICurrentUserSource<TUser>
{
    private readonly Lazy<TUser> lazyCurrentUser = LazyHelper.Create(() =>
        defaultCancellationTokenSource.RunSync(ct => userSource.GetUserAsync(currentUser.Name, ct)));

    public TUser CurrentUser => this.lazyCurrentUser.Value;

    public ICurrentUserSource<User> ToSimple()
    {
        return new CurrentUserSource<User>(currentUser, userSource.ToSimple());
    }
}