using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Storage.Inline.IdGenerator;

public class StateInstanceInlineIdGenerator() : InlineIdGenerator<StateInstance>(si => si.Workflow);