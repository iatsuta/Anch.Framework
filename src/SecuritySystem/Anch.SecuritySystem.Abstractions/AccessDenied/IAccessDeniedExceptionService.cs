using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.AccessDenied;

public interface IAccessDeniedExceptionService
{
    Exception GetAccessDeniedException(AccessResult.AccessDeniedResult accessDeniedResult);
}
