using System.Linq.Expressions;
using Anch.Core;

namespace Anch.SecuritySystem.UserSource;

public record UserSourceRunAsInfo<TUser>(PropertyAccessors<TUser, TUser?> RunAs)
{
	public UserSourceRunAsInfo(Expression<Func<TUser, TUser?>> runAsPath)
		: this(new PropertyAccessors<TUser, TUser?>(runAsPath))
	{
	}
}