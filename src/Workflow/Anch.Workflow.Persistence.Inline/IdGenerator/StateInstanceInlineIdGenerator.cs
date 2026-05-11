using Anch.IdentitySource;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Persistence.Inline.IdGenerator;

public class StateInstanceInlineIdGenerator(IIdentityInfoSource identityInfoSource) : InlineInstanceIdGenerator<StateInstance>(identityInfoSource, si => si.Workflow);