using Anch.Core.Auth;
using Anch.SecuritySystem.Attributes;

namespace Anch.SecuritySystem.UserSource;

/// <summary>Finds the user entity of the current user, ignoring any RunAs session.</summary>
public class WithoutRunAsCurrentUserSource<TUser>(ICurrentUser currentUser, [WithoutRunAs] IUserSource<TUser> userSource)
    : CurrentUserSource<TUser>(currentUser, userSource);