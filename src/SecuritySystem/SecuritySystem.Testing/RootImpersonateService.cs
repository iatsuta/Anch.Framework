using SecuritySystem.Services;

namespace SecuritySystem.Testing;

public class RootImpersonateService(RootImpersonateServiceState state) : ImpersonateService(state), IRootImpersonateService;