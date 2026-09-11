using System.Linq.Expressions;

using Anch.DependencyInjection;
using Anch.GenericQueryable.DependencyInjection;
using Anch.GenericQueryable.Services;

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;

namespace Anch.GenericQueryable.EntityFramework;

public class EfGenericQueryableSetup : GenericQueryableSetup, IEfGenericQueryableSetup
{
    private Action<IServiceCollection> initProviderAction = sc => sc.ReplaceScoped<IAsyncQueryProvider, VisitedEfQueryProvider>();

    private Action<IServiceCollection>? initVisitorAction;

    public IEfGenericQueryableSetup SetQueryProvider<TQueryProvider>()
        where TQueryProvider : class, IAsyncQueryProvider, IGenericQueryProvider
    {
        this.initProviderAction = sc => sc.ReplaceScoped<IAsyncQueryProvider, TQueryProvider>();

        return this;
    }

    public IEfGenericQueryableSetup SetVisitor<TVisitor>()
        where TVisitor : ExpressionVisitor
    {
        this.initVisitorAction = sc => sc.AddKeyedSingleton<ExpressionVisitor, TVisitor>(nameof(GenericQueryable));

        return this;
    }

    public IEfGenericQueryableSetup SetVisitor(ExpressionVisitor visitor)
    {
        this.initVisitorAction = sc => sc.AddKeyedSingleton(nameof(GenericQueryable), visitor);

        return this;
    }

    public override void Initialize(IServiceCollection services)
    {
        this.SetFetchService<EfFetchService>();
        this.SetTargetMethodExtractor<EfTargetMethodExtractor>();

        base.Initialize(services);

        this.initProviderAction.Invoke(services);
        this.initVisitorAction?.Invoke(services);
    }
}