

// ReSharper disable once CheckNamespace
namespace Anch.SecuritySystem;

/// <summary>Extensions for <see cref="ISecurityContextInfoSource"/>.</summary>
public static class SecurityContextInfoSourceExtensions
{
    extension(ISecurityContextInfoSource securityContextInfoSource)
    {
        /// <summary>Returns the CLR types of all registered security contexts.</summary>
        public IEnumerable<Type> GetSecurityContextTypes() =>
            securityContextInfoSource.SecurityContextInfoList.Select(info => info.Type);

        /// <summary>Returns the registration of a context type.</summary>
        /// <typeparam name="TSecurityContext">The security context type.</typeparam>
        public SecurityContextInfo GetSecurityContextInfo<TSecurityContext>()
            where TSecurityContext : ISecurityContext =>
            securityContextInfoSource.GetSecurityContextInfo(typeof(TSecurityContext));
    }
}