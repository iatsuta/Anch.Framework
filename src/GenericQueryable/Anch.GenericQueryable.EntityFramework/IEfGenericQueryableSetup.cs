using System.Linq.Expressions;

using Anch.GenericQueryable.DependencyInjection;
using Anch.GenericQueryable.Services;

using Microsoft.EntityFrameworkCore.Query;

namespace Anch.GenericQueryable.EntityFramework;

public interface IEfGenericQueryableSetup : IGenericQueryableSetup
{
    IEfGenericQueryableSetup SetQueryProvider<TQueryProvider>()
        where TQueryProvider : class, IAsyncQueryProvider, IGenericQueryProvider;

    IEfGenericQueryableSetup SetVisitor<TVisitor>()
        where TVisitor : ExpressionVisitor;
}