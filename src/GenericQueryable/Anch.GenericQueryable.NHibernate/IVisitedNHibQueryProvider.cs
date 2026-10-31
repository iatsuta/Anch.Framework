using System.Linq.Expressions;

using Anch.GenericQueryable.Services;

using NHibernate.Linq;

namespace Anch.GenericQueryable.NHibernate;

/// <summary>An NHibernate query provider that exposes the executor and applies an expression visitor to every query.</summary>
public interface IVisitedNHibQueryProvider : IGenericQueryProvider, INhQueryProvider
{
    /// <summary>The visitor applied to every query before execution; <c>null</c> for none.</summary>
    ExpressionVisitor? Visitor { get; set; }

    /// <summary>The executor that runs the ORM-independent operations.</summary>
    new IGenericQueryableExecutor Executor { get; set; }
}