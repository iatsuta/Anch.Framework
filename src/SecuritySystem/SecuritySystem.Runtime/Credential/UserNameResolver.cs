using CommonFramework.Auth;

using Microsoft.Extensions.DependencyInjection;

using SecuritySystem.UserSource;

// ReSharper disable once CheckNamespace
namespace SecuritySystem.Credential;

public class UserNameResolver<TUser>(
    ICurrentUser currentUser,
    [FromKeyedServices(ICurrentUser.ImpersonatedKey)] ICurrentUser impersonatedCurrentUser,
    IUserSource<TUser> userSource) : IUserNameResolver<TUser>
{
    private readonly IUserSource<User> simpleUserSource = userSource.ToSimple();

    public async ValueTask<string?> ResolveAsync(SecurityRuleCredential credential, CancellationToken cancellationToken)
    {
        return credential switch
        {
            SecurityRuleCredential.CustomUserSecurityRuleCredential customUserSecurityRuleCredential =>

                (await simpleUserSource.TryGetUserAsync(customUserSecurityRuleCredential.UserCredential, cancellationToken))?.Name,

            SecurityRuleCredential.CurrentUserWithRunAsCredential => currentUser.Name,

            SecurityRuleCredential.CurrentUserWithoutRunAsCredential => impersonatedCurrentUser.Name,

            SecurityRuleCredential.AnyUserCredential => null,

            _ => throw new ArgumentOutOfRangeException(nameof(credential))
        };
    }
}