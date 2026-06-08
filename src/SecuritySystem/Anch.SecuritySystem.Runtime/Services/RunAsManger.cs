using Anch.Core;
using Anch.Core.Auth;
using Anch.GenericRepository;
using Anch.SecuritySystem.UserSource;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.SecuritySystem.Services;

public class RunAsManager<TUser>(
    [FromKeyedServices(ICurrentUser.ImpersonatedKey)] ICurrentUser impersonatedCurrentUser,
    ISecuritySystemFactory securitySystemFactory,
    IEnumerable<IRunAsValidator> validators,
    IUserSource<TUser> userSource,
    UserSourceRunAsInfo<TUser> userSourceRunAsInfo,
    IGenericRepository genericRepository,
    IUserCredentialMatcher<TUser> userCredentialMatcher,
    IDefaultUserConverter<TUser> toDefaultUserConverter,
    IMissedUserErrorSource missedUserErrorSource,
    IDefaultCancellationTokenSource? defaultCancellationTokenSource = null) : IRunAsManager
    where TUser : class
{
    private readonly Lazy<TUser?> lazyNativeTryCurrentUser = new(() =>
        defaultCancellationTokenSource.RunSync(ct => userSource.TryGetUserAsync(impersonatedCurrentUser.Name, ct)));

    private TUser? NativeTryCurrentUser => this.lazyNativeTryCurrentUser.Value;

    private TUser NativeCurrentUser => this.NativeTryCurrentUser ??
                                       throw missedUserErrorSource.GetNotFoundException(typeof(TUser), impersonatedCurrentUser.Name);

    private TUser? NativeRunAsUser => this.NativeTryCurrentUser == null ? null : userSourceRunAsInfo.RunAs.Getter(this.NativeTryCurrentUser);

    public User? RunAsUser => this.NativeRunAsUser?.Pipe(toDefaultUserConverter.ConvertFunc);

    public async Task StartRunAsUserAsync(UserCredential userCredential, CancellationToken ct)
    {
        await this.CheckAccessAsync(ct);

        if (this.NativeRunAsUser is not null && userCredentialMatcher.IsMatch(userCredential, this.NativeRunAsUser))
        {
        }
        else if (userCredential == impersonatedCurrentUser.Name)
        {
            await this.FinishRunAsUserAsync(ct);
        }
        else
        {
            foreach (var runAsValidator in validators)
            {
                await runAsValidator.ValidateAsync(userCredential, ct);
            }

            await this.PersistRunAs(userCredential, ct);
        }
    }

    public async Task FinishRunAsUserAsync(CancellationToken ct)
    {
        await this.CheckAccessAsync(ct);

        await this.PersistRunAs(null, ct);
    }

    private async Task PersistRunAs(UserCredential? userCredential, CancellationToken ct)
    {
        var newRunAsUser = userCredential is null ? null : await userSource.GetUserAsync(userCredential, ct);

        if (this.NativeRunAsUser != newRunAsUser)
        {
            userSourceRunAsInfo.RunAs.Setter(this.NativeCurrentUser, newRunAsUser);

            await genericRepository.SaveAsync(this.NativeCurrentUser, ct);
        }
    }

    private Task CheckAccessAsync(CancellationToken ct) =>
        securitySystemFactory.Create(new SecurityRuleCredential.CurrentUserWithoutRunAsCredential())
            .CheckAccessAsync(SecurityRole.Administrator, ct);
}