// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public interface ISecuritySystem
{
    Task<bool> HasAccessAsync(DomainSecurityRule securityRule, CancellationToken cancellationToken = default);

    Task CheckAccessAsync(DomainSecurityRule securityRule, CancellationToken cancellationToken = default);
}