// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public interface ISecuritySystem
{
    Task<bool> HasAccessAsync(DomainSecurityRule securityRule, CancellationToken ct);

    Task CheckAccessAsync(DomainSecurityRule securityRule, CancellationToken ct);
}