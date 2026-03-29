using CommonFramework;
using SecuritySystem.Credential;
using SecuritySystem.UserSource;

namespace SecuritySystem.Services;

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