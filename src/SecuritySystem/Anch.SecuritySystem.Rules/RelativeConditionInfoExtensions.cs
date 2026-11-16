using System.Linq.Expressions;

namespace Anch.SecuritySystem;

/// <summary>Extensions for <see cref="RelativeConditionInfo"/>.</summary>
public static class RelativeConditionInfoExtensions
{
    /// <summary>Wraps a condition on a related object.</summary>
    /// <param name="condition">The condition.</param>
    /// <typeparam name="TRelativeDomainObject">The type of the related object.</typeparam>
    public static RelativeConditionInfo<TRelativeDomainObject> ToInfo<TRelativeDomainObject>(this Expression<Func<TRelativeDomainObject, bool>> condition)
    {
        return new RelativeConditionInfo<TRelativeDomainObject>(condition);
    }
}