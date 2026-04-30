using Anch.Workflow.Domain.Runtime;

namespace Anch.Workflow.Storage.Inline.IdGenerator;

public class StateInstanceInlineIdGenerator() : InlineIdGenerator<StateInstance>(si => si.Workflow);