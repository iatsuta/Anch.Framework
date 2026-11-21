using Anch.Core;

using Microsoft.Extensions.DependencyInjection;

namespace Anch.Workflow.IntegrationTests._Base;

public class TestBase(IServiceProvider rootServiceProvider)
{
    //protected override IServiceCollection CreateServices()
    //{
    //    return base.CreateServices()

    //        .AddSingleton(TestSystemConfigurationHelper.BuildConfiguration("Data Source=TestSystem.sqlite"))
    //        .AddSingletonFrom((global::NHibernate.Cfg.Configuration cfg) => cfg.BuildSessionFactory())
    //        .AddScopedFrom((ISessionFactory sessionFactory) => sessionFactory.OpenSession())

    //        .ReplaceScoped<IInstanceIdGenerator<WorkflowInstance>, WorkflowInstanceInlineIdGenerator>()
    //        .ReplaceScoped<IInstanceIdGenerator<StateInstance>, StateInstanceInlineIdGenerator>()
    //        .ReplaceScoped<ISpecificWorkflowExternalStorageSource, InlineSpecificWorkflowExternalStorageSource>();
    //}

    //protected override IServiceCollection CreateServices(IServiceCollection services)
    //{
    //    return base.CreateServices(services)
    //        .ReplaceScoped<IInstanceIdGenerator<WorkflowInstance>, WorkflowInstanceInlineIdGenerator>()
    //        .ReplaceScoped<IInstanceIdGenerator<StateInstance>, StateInstanceInlineIdGenerator>()
    //        .ReplaceScoped<IWorkflowRepositoryFactory, InlineWorkflowRepositoryFactory>();
    //}

    protected async Task<TResult> EvaluateScope<TResult>(Func<IServiceProvider, Task<TResult>> func)
    {
        await using var scope = rootServiceProvider.CreateAsyncScope();

        var result = await func(scope.ServiceProvider);

        return result;
    }

    protected Task EvaluateScope(Func<IServiceProvider, Task> action) => this.EvaluateScope(action.ToDefaultTask());

    protected async Task EvaluateAssertScope(Func<IServiceProvider, Task> action)
    {
        await this.EvaluateScope(async serviceProvider =>
        {
            await action(serviceProvider);

            var events = await serviceProvider.WorkflowRepository.GetWaitEvents().ToListAsync();

            Assert.Empty(events);
        });
    }
}