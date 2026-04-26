using Anch.Core;
using Anch.Core.Auth;
using Anch.SecuritySystem.UserSource;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Services;

public class UserNameResolver(IEnumerable<IUserSource> userSourceList) : IUserNameResolver
{
    public async ValueTask<string> GetUserNameAsync(UserCredential userCredential, CancellationToken cancellationToken)
    {
        switch (userCredential)
        {
            case UserCredential.NamedUserCredential { Name: var name }:
                return name;

            case UserCredential.IdentUserCredential { Identity: var identity }:
            {
                var request =

                    await userSourceList
                        .ToAsyncEnumerable()
                        .Select((userSource, ct) => userSource.ToSimple().TryGetUserAsync(userCredential, ct))
                        .Where(user => user != null)
                        .Select(user => user!.Name)
                        .Distinct()
                        .ToArrayAsync(cancellationToken);

                return request.Single(
                    () => new SecuritySystemException($"{nameof(UserCredential)} with id {identity.GetId()} not found"),
                    names => new SecuritySystemException($"More one {nameof(UserCredential)} with id {identity.GetId()}: {names.Join(", ", name => $"\"{name}\"")}"));
            }

            default: throw new ArgumentOutOfRangeException(nameof(userCredential));
        }
    }
}
public class UserNameResolver<TUser>(
    ICurrentUser currentUser,
    [FromKeyedServices(ICurrentUser.ImpersonatedKey)] ICurrentUser impersonatedCurrentUser,
    IUserSource<TUser> userSource) : IUserNameResolver<TUser>
{
    private readonly IUserSource<User> simpleUserSource = userSource.ToSimple();

    public async ValueTask<string?> GetUserNameAsync(SecurityRuleCredential credential, CancellationToken cancellationToken)
    {
        return credential switch
        {
            SecurityRuleCredential.CustomUserSecurityRuleCredential customUserSecurityRuleCredential =>

                (await this.simpleUserSource.GetUserAsync(customUserSecurityRuleCredential.UserCredential, cancellationToken)).Name,

            SecurityRuleCredential.CurrentUserWithRunAsCredential => currentUser.Name,

            SecurityRuleCredential.CurrentUserWithoutRunAsCredential => impersonatedCurrentUser.Name,

            SecurityRuleCredential.AnyUserCredential => null,

            _ => throw new ArgumentOutOfRangeException(nameof(credential))
        };
    }
}