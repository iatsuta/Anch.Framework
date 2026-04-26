using System.Linq.Expressions;

using Anch.Core;
using Anch.RelativePath;
using Anch.SecuritySystem.DiTests.DomainObjects;

namespace Anch.SecuritySystem.DiTests.Services;

public class TestCheckboxConditionFactory<TDomainObject>(IRelativeDomainPathInfo<TDomainObject, Employee> pathToEmployeeInfo)
    : IFactory<Expression<Func<TDomainObject, bool>>>
{
    public Expression<Func<TDomainObject, bool>> Create()
    {
        return pathToEmployeeInfo.CreateCondition(employee => employee.TestCheckbox);
    }
}
