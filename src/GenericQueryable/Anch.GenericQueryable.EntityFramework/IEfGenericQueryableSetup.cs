using System.Linq.Expressions;

using Anch.DependencyInjection;
using Anch.GenericQueryable.DependencyInjection;
using Anch.GenericQueryable.Services;

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anch.GenericQueryable.EntityFramework;

public static class GenericQueryableSetupExtensions
{
    public static IGenericQueryableSetup SetQueryProvider<TQueryProvider>(this IGenericQueryableSetup setup)
        where TQueryProvider : class, IAsyncQueryProvider, IGenericQueryProvider
    {
        return setup.AddServices(sc =>

            sc.ReplaceScoped<IAsyncQueryProvider, TQueryProvider>());
    }

    public static IGenericQueryableSetup SetVisitor<TVisitor>(this IGenericQueryableSetup setup)
        where TVisitor : ExpressionVisitor
    {
        return setup.AddServices(sc =>

            sc.Replace(ServiceDescriptor.KeyedSingleton<ExpressionVisitor, TVisitor>(nameof(GenericQueryable))));
    }
}