using System.Linq.Expressions;

using Anch.SecuritySystem.UserSource;

namespace Anch.SecuritySystem.DependencyInjection;

public interface IUserSourceSetup<TUser>
{
    IUserSourceSetup<TUser> SetName(Expression<Func<TUser, string>> namePath);

    IUserSourceSetup<TUser> SetFilter(Expression<Func<TUser, bool>> filterPath);

    IUserSourceSetup<TUser> SetRunAs(Expression<Func<TUser, TUser?>> runAsPath);

    IUserSourceSetup<TUser> SetMissedService<TService>()
        where TService : IMissedUserService<TUser>;
}