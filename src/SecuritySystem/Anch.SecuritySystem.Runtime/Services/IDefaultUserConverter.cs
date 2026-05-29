using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

public interface IDefaultUserConverter<TUser>
{
    Expression<Func<TUser, User>> ConvertExpression { get; }

    Func<TUser, User> ConvertFunc { get; }
}