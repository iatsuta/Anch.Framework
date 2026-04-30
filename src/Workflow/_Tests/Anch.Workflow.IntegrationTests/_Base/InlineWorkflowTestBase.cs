using Framework.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using FluentAssertions;

using SyncWorkflow.Storage;
using SyncWorkflow.Domain.Runtime;
using SyncWorkflow.Storage.Inline.IdGenerator;
using SyncWorkflow.Storage.Inline;
using NHibernate;
using SyncWorkflow.IntegrationTests.NHib;

namespace SyncWorkflow.Tests;

public class InlineWorkflowTestBase : MultiScopeWorkflowTestBase
{
    protected override IServiceCollection CreateServices()
    {
        return base.CreateServices()

            .AddSingleton(TestSystemConfigurationHelper.BuildConfiguration("Data Source=TestSystem.sqlite"))
            .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())
            .AddScopedFrom((ISessionFactory sessionFactory) => sessionFactory.OpenSession())

            .ReplaceScoped<IInstanceIdGenerator<WorkflowInstance>, WorkflowInstanceInlineIdGenerator>()
            .ReplaceScoped<IInstanceIdGenerator<StateInstance>, StateInstanceInlineIdGenerator>()
            .ReplaceScoped<ISpecificWorkflowExternalStorageSource, InlineSpecificWorkflowExternalStorageSource>();
    }

    protected async Task<TResult> EvaluateScope<TResult>(Func<InlineScope, Task<TResult>> func)
    {
        await using var scope = this.RootServiceProvider.CreateAsyncScope();

        var inlineScope = new InlineScope(scope.ServiceProvider);

        var result = await func(inlineScope);

        await inlineScope.Storage.FlushChanges();

        return result;
    }

    protected async Task EvaluateScope(Func<InlineScope, Task> action)
    {
        await this.EvaluateScope(async scope =>
        {
            await action(scope);

            return default(object);
        });
    }


    protected async Task EvaluateAssertScope(Func<InlineScope, Task> action)
    {
        await this.EvaluateScope(async scope =>
        {
            await action(scope);

            var events = await scope.Storage.GetWaitEvents();

            events.Should().BeEmpty();
        });
    }
}