using System.Linq.Expressions;

using Anch.SecuritySystem.UserSource;

namespace Anch.SecuritySystem.DependencyInjection;

/// <summary>Fluent configuration of how the application's user entity is read.</summary>
/// <typeparam name="TUser">The user entity type.</typeparam>
public interface IUserSourceSetup<TUser>
{
    /// <summary>Sets the property that holds the user name.</summary>
    /// <param name="namePath">Selects the name.</param>
    IUserSourceSetup<TUser> SetName(Expression<Func<TUser, string>> namePath);

    /// <summary>Sets the condition that selects the active users.</summary>
    /// <param name="filterPath">The condition.</param>
    IUserSourceSetup<TUser> SetFilter(Expression<Func<TUser, bool>> filterPath);

    /// <summary>Sets the property that holds the user being run as.</summary>
    /// <param name="runAsPath">Selects the RunAs user of a user.</param>
    IUserSourceSetup<TUser> SetRunAs(Expression<Func<TUser, TUser?>> runAsPath);

    /// <summary>Sets the service that supplies a user when none is found.</summary>
    /// <typeparam name="TService">The service type.</typeparam>
    IUserSourceSetup<TUser> SetMissedService<TService>()
        where TService : IMissedUserService<TUser>;
}