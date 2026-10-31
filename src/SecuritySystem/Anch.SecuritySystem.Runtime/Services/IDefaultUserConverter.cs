using System.Linq.Expressions;

namespace Anch.SecuritySystem.Services;

/// <summary>Converts a user entity into the simple <see cref="User"/> record.</summary>
public interface IDefaultUserConverter<TUser>
{
    Expression<Func<TUser, User>> ConvertExpression { get; }

    Func<TUser, User> ConvertFunc { get; }
}