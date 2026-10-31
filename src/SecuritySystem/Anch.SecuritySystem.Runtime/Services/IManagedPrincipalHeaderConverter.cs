using System.Linq.Expressions;

using Anch.SecuritySystem.ExternalSystem.Management;

namespace Anch.SecuritySystem.Services;

/// <summary>Converts a principal entity into a managed principal header.</summary>
public interface IManagedPrincipalHeaderConverter<TPrincipal>
{
    Expression<Func<TPrincipal, ManagedPrincipalHeader>> ConvertExpression { get; }

    ManagedPrincipalHeader Convert(TPrincipal principal);
}