using System.Linq.Expressions;

namespace SecuritySystem;

public static class RelativeConditionInfoExtensions
{
    public static RelativeConditionInfo<TRelativeDomainObject> ToInfo<TRelativeDomainObject>(this Expression<Func<TRelativeDomainObject, bool>> condition)
    {
        return new RelativeConditionInfo<TRelativeDomainObject>(condition);
    }
}