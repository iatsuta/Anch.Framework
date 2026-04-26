using Anch.Core.Auth;
using Anch.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anch.SecuritySystem.Testing.DependencyInjection;

public class SecuritySystemTestingSetup : ISecuritySystemTestingSetup, IServiceInitializer
{
    private Type evaluatorType = typeof(TestingEvaluator<>);

    private Func<IServiceProvider, TestRootUserInfo> getTestRootUserInfoFunc = _ => TestRootUserInfo.Default;

    public ISecuritySystemTestingSetup SetEvaluator(Type newEvaluatorType)
    {
        this.evaluatorType = newEvaluatorType;

        return this;
    }

    public ISecuritySystemTestingSetup SetTestRootUserInfo(Func<IServiceProvider, TestRootUserInfo> getInfo)
    {
        this.getTestRootUserInfoFunc = getInfo;

        return this;
    }

    public void Initialize(IServiceCollection services)
    {
        services

            .AddSingleton<RootImpersonateServiceState>()
            .AddSingleton<IRootImpersonateService, RootImpersonateService>()

            .Replace(ServiceDescriptor.KeyedScoped<ICurrentUser, TestingRawCurrentUser>(ICurrentUser.RawKey))
            .Replace(ServiceDescriptor.KeyedSingleton<ICurrentUser, TestingDefaultCurrentUser>(ICurrentUser.DefaultKey))

            .AddScoped(typeof(UserCredentialManager))

            .AddSingleton<RootAuthManager>()
            .AddSingleton(AdministratorsRoleList.Default)
            .AddSingleton(this.getTestRootUserInfoFunc)
            .AddSingleton(typeof(ITestingEvaluator<>), this.evaluatorType);
    }
}