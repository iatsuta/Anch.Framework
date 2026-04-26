using Anch.SecuritySystem.Services;

namespace Anch.SecuritySystem.Testing;

public class RootImpersonateService(RootImpersonateServiceState state) : ImpersonateService(state), IRootImpersonateService;