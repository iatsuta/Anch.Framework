using System.Linq.Expressions;

using Anch.GenericQueryable.Services;

using NHibernate.Linq;

namespace Anch.GenericQueryable.NHibernate;

public interface IVisitedNHibQueryProvider : IGenericQueryProvider, INhQueryProvider
{
    ExpressionVisitor? Visitor { get; set; }

    new IGenericQueryableExecutor Executor { get; set; }
}