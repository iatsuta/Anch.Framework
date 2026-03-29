using CommonFramework.Auth;

using SecuritySystem.Attributes;

namespace SecuritySystem.UserSource;

public class WithoutRunAsCurrentUserSource<TUser>(ICurrentUser currentUser, [WithoutRunAs] IUserSource<TUser> userSource)
    : CurrentUserSource<TUser>(currentUser, userSource);