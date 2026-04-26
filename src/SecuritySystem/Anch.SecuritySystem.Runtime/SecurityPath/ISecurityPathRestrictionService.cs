// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

public interface ISecurityPathRestrictionService
{
    SecurityPath<TDomainObject> ApplyRestriction<TDomainObject>(SecurityPath<TDomainObject>? securityPath, SecurityPathRestriction restriction);
}