using Anch.Core.Auth;
using Anch.SecuritySystem.Attributes;

namespace Anch.SecuritySystem.UserSource;

public class WithoutRunAsCurrentUserSource<TUser>(ICurrentUser currentUser, [WithoutRunAs] IUserSource<TUser> userSource)
    : CurrentUserSource<TUser>(currentUser, userSource);