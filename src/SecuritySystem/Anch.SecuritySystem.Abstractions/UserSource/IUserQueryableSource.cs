namespace Anch.SecuritySystem.UserSource;

/// <summary>Provides a query over the users that match a credential.</summary>
/// <typeparam name="TUser">The user entity type.</typeparam>
public interface IUserQueryableSource<out TUser>
{
    /// <summary>Returns the query for the credential.</summary>
    /// <param name="userCredential">The user to look for.</param>
    IQueryable<TUser> GetQueryable(UserCredential userCredential);

    /// <summary>Returns a view that yields the simple <see cref="User"/> record.</summary>
    IUserQueryableSource<User> ToSimple();
}