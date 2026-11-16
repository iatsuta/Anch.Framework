using Anch.SecuritySystem.Providers;

namespace Anch.SecuritySystem.AccessDenied;

/// <summary>Creates the exception to throw when access is denied; replace it with <c>SetAccessDeniedExceptionService</c> to throw an application-specific exception.</summary>
public interface IAccessDeniedExceptionService
{
    /// <summary>Creates the exception for a denial.</summary>
    /// <param name="accessDeniedResult">Describes what was denied.</param>
    Exception GetAccessDeniedException(AccessResult.AccessDeniedResult accessDeniedResult);
}
