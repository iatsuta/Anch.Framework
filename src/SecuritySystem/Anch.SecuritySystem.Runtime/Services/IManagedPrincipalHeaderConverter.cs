using System.Linq.Expressions;
using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Services;

public interface IManagedPrincipalHeaderConverter<TPrincipal>
{
    Expression<Func<TPrincipal, ManagedPrincipalHeader>> ConvertExpression { get; }

    ManagedPrincipalHeader Convert(TPrincipal principal);
}