using System.Linq.Expressions;

using Anch.Core;
using Anch.RelativePath;
// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public class RelativeConditionFactory<TDomainObject, TRelativeDomainObject>(
    RelativeConditionInfo<TRelativeDomainObject> conditionInfo,
    IRelativeDomainPathInfo<TDomainObject, TRelativeDomainObject>? relativeDomainPathInfo = null)
    : IFactory<Expression<Func<TDomainObject, bool>>?>
{
    public Expression<Func<TDomainObject, bool>>? Create() => relativeDomainPathInfo?.CreateCondition(conditionInfo.Condition);
}

public class RequiredRelativeConditionFactory<TDomainObject, TRelativeDomainObject>(
    RelativeConditionInfo<TRelativeDomainObject> conditionInfo,
    IRelativeDomainPathInfo<TDomainObject, TRelativeDomainObject> relativeDomainPathInfo)
    : IFactory<Expression<Func<TDomainObject, bool>>>
{
    public Expression<Func<TDomainObject, bool>> Create() => relativeDomainPathInfo.CreateCondition(conditionInfo.Condition);
}
