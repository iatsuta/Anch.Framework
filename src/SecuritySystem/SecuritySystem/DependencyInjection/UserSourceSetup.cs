using SecuritySystem.UserSource;
using System.Linq.Expressions;

using CommonFramework.VisualIdentitySource.DependencyInjection;

namespace SecuritySystem.DependencyInjection;

public class UserSourceSetup<TUser> : IUserSourceSetup<TUser>
{
	public Action<IVisualIdentitySourceSetup>? VisualIdentitySetupAction { get; private set; }

	public Expression<Func<TUser, bool>> FilterPath { get; private set; } = _ => true;

	public Expression<Func<TUser, TUser?>>? RunAsPath { get; private set; }

	public Type MissedUserServiceType { get; private set; } = typeof(ErrorMissedUserService<TUser>);

	public IUserSourceSetup<TUser> SetName(Expression<Func<TUser, string>> namePath)
	{
		this.VisualIdentitySetupAction = s => s.SetName(namePath);

		return this;
	}

	public IUserSourceSetup<TUser> SetFilter(Expression<Func<TUser, bool>> filterPath)
	{
		this.FilterPath = filterPath;

		return this;
	}

	public IUserSourceSetup<TUser> SetRunAs(Expression<Func<TUser, TUser?>> runAsPath)
	{
		this.RunAsPath = runAsPath;

		return this;
	}

	public IUserSourceSetup<TUser> SetMissedService<TService>() where TService : IMissedUserService<TUser>
	{
		this.MissedUserServiceType = typeof(TService);

		return this;
	}
}