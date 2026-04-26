namespace Anch.Core;

public interface IQueryableInjector<T>
{
    IQueryable<T> Inject(IQueryable<T> queryable);
}