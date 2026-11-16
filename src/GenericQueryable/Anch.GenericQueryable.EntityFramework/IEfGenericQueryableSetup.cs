using System.Linq.Expressions;

using Anch.DependencyInjection;
using Anch.GenericQueryable.DependencyInjection;
using Anch.GenericQueryable.Services;

using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Anch.GenericQueryable.EntityFramework;

/// <summary>Entity Framework specific settings of GenericQueryable.</summary>
public static class GenericQueryableSetupExtensions
{
    /// <summary>Replaces the EF query provider with a custom one that also implements <c>IGenericQueryProvider</c>.</summary>
    /// <param name="setup">The setup.</param>
    /// <typeparam name="TQueryProvider">The query provider type.</typeparam>
    public static IGenericQueryableSetup SetQueryProvider<TQueryProvider>(this IGenericQueryableSetup setup)
        where TQueryProvider : class, IAsyncQueryProvider, IGenericQueryProvider
    {
        return setup.AddServices(sc =>

            sc.ReplaceScoped<IAsyncQueryProvider, TQueryProvider>());
    }

    /// <summary>Sets an expression visitor that is applied to every query before it is executed.</summary>
    /// <param name="setup">The setup.</param>
    /// <typeparam name="TVisitor">The visitor type.</typeparam>
    public static IGenericQueryableSetup SetVisitor<TVisitor>(this IGenericQueryableSetup setup)
        where TVisitor : ExpressionVisitor
    {
        return setup.AddServices(sc =>

            sc.Replace(ServiceDescriptor.KeyedSingleton<ExpressionVisitor, TVisitor>(nameof(GenericQueryable))));
    }
}