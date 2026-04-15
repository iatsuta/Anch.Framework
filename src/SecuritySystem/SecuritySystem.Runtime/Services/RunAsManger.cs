using CommonFramework;
using CommonFramework.Auth;
using CommonFramework.GenericRepository;
using Microsoft.Extensions.DependencyInjection;
using SecuritySystem.UserSource;

namespace SecuritySystem.Services;

public class RunAsManager<TUser>(
    [FromKeyedServices(ICurrentUser.ImpersonatedKey)]ICurrentUser impersonatedCurrentUser,
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

    public async Task StartRunAsUserAsync(UserCredential userCredential, CancellationToken cancellationToken)
    {
        await this.CheckAccessAsync(cancellationToken);

        if (this.NativeRunAsUser is not null && userCredentialMatcher.IsMatch(userCredential, this.NativeRunAsUser))
        {
        }
        else if (userCredential == impersonatedCurrentUser.Name)
        {
            await this.FinishRunAsUserAsync(cancellationToken);
        }
        else
        {
            foreach (var runAsValidator in validators)
            {
                await runAsValidator.ValidateAsync(userCredential, cancellationToken);
            }

            await this.PersistRunAs(userCredential, cancellationToken);
        }
    }

    public async Task FinishRunAsUserAsync(CancellationToken cancellationToken)
    {
        await this.CheckAccessAsync(cancellationToken);

        await this.PersistRunAs(null, cancellationToken);
    }

    private async Task PersistRunAs(UserCredential? userCredential, CancellationToken cancellationToken)
    {
        var newRunAsUser = userCredential is null ? null : await userSource.GetUserAsync(userCredential, cancellationToken);

        if (this.NativeRunAsUser != newRunAsUser)
        {
            userSourceRunAsInfo.RunAs.Setter(this.NativeCurrentUser, newRunAsUser);

            await genericRepository.SaveAsync(this.NativeCurrentUser, cancellationToken);
        }
    }

    private ValueTask CheckAccessAsync(CancellationToken cancellationToken) =>
        securitySystemFactory.Create(new SecurityRuleCredential.CurrentUserWithoutRunAsCredential())
            .CheckAccessAsync(SecurityRole.Administrator, cancellationToken);
}