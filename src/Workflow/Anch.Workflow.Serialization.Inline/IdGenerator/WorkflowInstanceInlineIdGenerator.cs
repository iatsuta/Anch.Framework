using Anch.IdentitySource;
using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Serialization.Inline.IdGenerator;

public class WorkflowInstanceInlineIdGenerator(IIdentityInfoSource identityInfoSource) : InlineInstanceIdGenerator<WorkflowInstance>(identityInfoSource, wi => wi);