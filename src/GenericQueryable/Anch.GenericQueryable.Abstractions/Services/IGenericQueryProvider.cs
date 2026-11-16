namespace Anch.GenericQueryable.Services;

/// <summary>An <see cref="IQueryProvider"/> that knows how to execute the ORM-independent operations; implemented by the ORM adapters.</summary>
public interface IGenericQueryProvider : IQueryProvider
{
    /// <summary>The executor that runs <c>GenericXxx</c> calls and applies fetch rules for queries of this provider.</summary>
    IGenericQueryableExecutor Executor { get; }
}