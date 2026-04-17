using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage.Inline.IdGenerator;

public class WorkflowInstanceInlineIdGenerator() : InlineIdGenerator<WorkflowInstance>(wi => wi);