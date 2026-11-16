namespace Anch.SecuritySystem.SecurityAccessor;

/// <summary>The base of the visitors of accessor data.</summary>
public abstract class SecurityAccessorDataVisitor
{
    public virtual SecurityAccessorData Visit(SecurityAccessorData baseData)
    {
        return baseData switch
        {
            SecurityAccessorData.FixedSecurityAccessorData result => this.Visit(result),

            SecurityAccessorData.AndSecurityAccessorData result => this.Visit(result),

            SecurityAccessorData.OrSecurityAccessorData result => this.Visit(result),

            SecurityAccessorData.InfinitySecurityAccessorData result => this.Visit(result),

            SecurityAccessorData.NegateSecurityAccessorData result => this.Visit(result),

            _ => throw new ArgumentOutOfRangeException(nameof(baseData))
        };
    }

    public virtual SecurityAccessorData Visit(SecurityAccessorData.FixedSecurityAccessorData result)
    {
        return result;
    }

    public virtual SecurityAccessorData Visit(SecurityAccessorData.AndSecurityAccessorData result)
    {
        return result;
    }

    public virtual SecurityAccessorData Visit(SecurityAccessorData.OrSecurityAccessorData result)
    {
        return result;
    }

    public virtual SecurityAccessorData Visit(SecurityAccessorData.InfinitySecurityAccessorData result)
    {
        return result;
    }

    public virtual SecurityAccessorData Visit(SecurityAccessorData.NegateSecurityAccessorData result)
    {
        return result;
    }
}
