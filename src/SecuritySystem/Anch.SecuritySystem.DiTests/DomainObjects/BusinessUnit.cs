using Anch.SecuritySystem.DiTests.DomainObjects._Base;

namespace Anch.SecuritySystem.DiTests.DomainObjects;

public class BusinessUnit : PersistentDomainObjectBase, ISecurityContext
{
    public BusinessUnit? Parent { get; set; }

    public IEnumerable<BusinessUnit> Children { get; set; } = new List<BusinessUnit>();
}
