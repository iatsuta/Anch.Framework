namespace Anch.GenericQueryable.Services;

public interface IGenericQueryProvider : IQueryProvider
{
    IGenericQueryableExecutor Executor { get; }
}