using System.Linq.Expressions;
using Anch.SecuritySystem.UserSource;

namespace Anch.SecuritySystem.Services;

public interface IDefaultUserConverter<TUser>
{
	Expression<Func<TUser, User>> ConvertExpression { get; }

	Func<TUser, User> ConvertFunc { get; }
}