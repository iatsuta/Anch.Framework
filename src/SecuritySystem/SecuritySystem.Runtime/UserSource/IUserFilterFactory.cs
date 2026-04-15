using System.Linq.Expressions;

namespace SecuritySystem.UserSource;

public interface IUserFilterFactory<TUser>
{
	Expression<Func<TUser, bool>> CreateFilter(UserCredential userCredential);
}