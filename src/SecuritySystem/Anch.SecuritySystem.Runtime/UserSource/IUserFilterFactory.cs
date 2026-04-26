using System.Linq.Expressions;

namespace Anch.SecuritySystem.UserSource;

public interface IUserFilterFactory<TUser>
{
	Expression<Func<TUser, bool>> CreateFilter(UserCredential userCredential);
}