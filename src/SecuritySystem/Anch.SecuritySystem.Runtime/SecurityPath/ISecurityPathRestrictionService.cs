// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Applies the restrictions of a role to a security path.</summary>
public interface ISecurityPathRestrictionService
{
    SecurityPath<TDomainObject> ApplyRestriction<TDomainObject>(SecurityPath<TDomainObject>? securityPath, SecurityPathRestriction restriction);
}