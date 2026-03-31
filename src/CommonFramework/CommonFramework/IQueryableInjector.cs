namespace CommonFramework;

public interface IQueryableInjector<T>
{
    IQueryable<T> Inject(IQueryable<T> queryable);
}