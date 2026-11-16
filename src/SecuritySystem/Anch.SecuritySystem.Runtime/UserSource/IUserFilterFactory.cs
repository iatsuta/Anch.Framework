using System.Linq.Expressions;

namespace Anch.SecuritySystem.UserSource;

/// <summary>Builds filters that find users by credential.</summary>
public interface IUserFilterFactory<TUser>
{
    Expression<Func<TUser, bool>> CreateFilter(UserCredential userCredential);
}