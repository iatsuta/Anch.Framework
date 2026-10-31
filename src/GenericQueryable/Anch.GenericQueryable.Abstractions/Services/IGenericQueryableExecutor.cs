using System.Linq.Expressions;

using Anch.GenericQueryable.Fetching;

namespace Anch.GenericQueryable.Services;

/// <summary>Executes ORM-independent operation calls by redirecting them to the real ORM methods.</summary>
public interface IGenericQueryableExecutor
{
    /// <summary>Applies fetch rules for this executor.</summary>
    IFetchService FetchService { get; }

    /// <summary>Redirects the call to the target ORM method and invokes it.</summary>
    /// <param name="callExpression">The call, for example <c>() =&gt; source.GenericToListAsync(ct)</c>.</param>
    /// <typeparam name="TResult">Result type of the call.</typeparam>
    TResult Execute<TResult>(Expression<Func<TResult>> callExpression);
}