using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage.Inline.IdGenerator;

public class WorkflowInstanceInlineIdGenerator() : InlineIdGenerator<WorkflowInstance>(wi => wi);