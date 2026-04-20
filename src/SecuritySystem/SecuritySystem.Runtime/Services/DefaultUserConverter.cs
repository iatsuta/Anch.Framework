using SecuritySystem.UserSource;

using System.Linq.Expressions;

using CommonFramework;
using CommonFramework.ExpressionEvaluate;
using CommonFramework.VisualIdentitySource;

namespace SecuritySystem.Services;
public class DefaultUserConverter<TUser>(
    ISecurityIdentityManager<TUser> securityIdentityManager,
    IVisualIdentityInfo<TUser> visualIdentityInfo) : IDefaultUserConverter<TUser>
{
    private readonly Tuple<Expression<Func<TUser, User>>, Func<TUser, User>> convertData = FuncHelper.Create(() =>
    {
        var convertExpr = ExpressionEvaluateHelper.InlineEvaluate<Func<TUser, User>>(ee =>
            user => new User(ee.Evaluate(visualIdentityInfo.Name.Path, user), ee.Evaluate(securityIdentityManager.SecurityIdentityExpression, user)));

        return new Tuple<Expression<Func<TUser, User>>, Func<TUser, User>>(convertExpr, convertExpr.Compile());
    }).Invoke();

    public Expression<Func<TUser, User>> ConvertExpression => this.convertData.Item1;

    public Func<TUser, User> ConvertFunc => this.convertData.Item2;
}