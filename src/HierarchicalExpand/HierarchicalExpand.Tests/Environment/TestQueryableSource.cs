using CommonFramework.GenericRepository;

namespace HierarchicalExpand.Tests.Environment;

public class TestQueryableSource : IQueryableSource
{
    public IQueryableSource InnerSource { get; private set; } = Substitute.For<IQueryableSource>();

    public void Reset()
    {
        this.InnerSource = Substitute.For<IQueryableSource>();
    }

    public IQueryable<TDomainObject> GetQueryable<TDomainObject>()
        where TDomainObject : class
    {
        return this.InnerSource.GetQueryable<TDomainObject>();
    }
}