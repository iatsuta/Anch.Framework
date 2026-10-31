using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Testing;

/// <summary>The impersonation service of the testing root user.</summary>
public class RootImpersonateService(RootImpersonateServiceState state) : ImpersonateService(state), IRootImpersonateService;