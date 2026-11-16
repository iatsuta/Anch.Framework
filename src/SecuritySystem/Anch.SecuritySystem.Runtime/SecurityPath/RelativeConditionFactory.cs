using System.Linq.Expressions;

using Anch.Core;
using Anch.RelativePath;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Turns a condition on a related object into a condition on the domain object, when the relative path exists.</summary>
public class RelativeConditionFactory<TDomainObject, TRelativeDomainObject>(
    RelativeConditionInfo<TRelativeDomainObject> conditionInfo,
    IRelativeDomainPathInfo<TDomainObject, TRelativeDomainObject>? relativeDomainPathInfo = null)
    : IFactory<Expression<Func<TDomainObject, bool>>?>
{
    public Expression<Func<TDomainObject, bool>>? Create() => relativeDomainPathInfo?.CreateCondition(conditionInfo.Condition);
}

/// <summary>Turns a condition on a related object into a condition on the domain object; the relative path is required.</summary>
public class RequiredRelativeConditionFactory<TDomainObject, TRelativeDomainObject>(
    RelativeConditionInfo<TRelativeDomainObject> conditionInfo,
    IRelativeDomainPathInfo<TDomainObject, TRelativeDomainObject> relativeDomainPathInfo)
    : IFactory<Expression<Func<TDomainObject, bool>>>
{
    public Expression<Func<TDomainObject, bool>> Create() => relativeDomainPathInfo.CreateCondition(conditionInfo.Condition);
}
