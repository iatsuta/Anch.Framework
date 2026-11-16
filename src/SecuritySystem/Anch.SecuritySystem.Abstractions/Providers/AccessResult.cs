namespace Anch.SecuritySystem.Providers;

/// <summary>Result of an access check: granted or denied (with the details of the denial). Results combine with <see cref="And"/> and <see cref="Or"/>.</summary>
public abstract record AccessResult
{
    /// <summary>Access is granted.</summary>
    public record AccessGrantedResult : AccessResult
    {
        public override AccessResult Or(AccessResult otherAccessResult) => this;

        public override AccessResult And(AccessResult otherAccessResult) => otherAccessResult;


        public static readonly AccessGrantedResult Default = new();
    }

    /// <summary>Access is denied; carries what is needed to build the error.</summary>
    public record AccessDeniedResult : AccessResult
    {
        /// <summary>The rule that was not satisfied.</summary>
        public SecurityRule? SecurityRule { get; init; }

        /// <summary>A message that replaces the default error text.</summary>
        public string? CustomMessage { get; init; }

        /// <summary>The object access to which was denied, with its type.</summary>
        public (object DomainObject, Type DomainObjectType)? DomainObjectInfo { get; init; }


        public override AccessResult Or(AccessResult otherAccessResult) => otherAccessResult;

        public override AccessResult And(AccessResult otherAccessResult) => this;


        /// <summary>Creates a denial for an object.</summary>
        /// <param name="domainObject">The object access to which was denied.</param>
        /// <param name="securityRule">The rule that was not satisfied.</param>
        /// <typeparam name="TDomainObject">The type of the object.</typeparam>
        public static AccessDeniedResult Create<TDomainObject>(TDomainObject domainObject, SecurityRule? securityRule = null) =>
            new() { DomainObjectInfo = (domainObject, typeof(TDomainObject))!, SecurityRule = securityRule };

        public static readonly AccessDeniedResult Default = new();
    }

    /// <summary>Both results must be granted: returns a denial if either is denied.</summary>
    /// <param name="otherAccessResult">The other result.</param>
    public abstract AccessResult And(AccessResult otherAccessResult);

    /// <summary>Either result is enough: returns a grant if either is granted.</summary>
    /// <param name="otherAccessResult">The other result.</param>
    public abstract AccessResult Or(AccessResult otherAccessResult);
}
