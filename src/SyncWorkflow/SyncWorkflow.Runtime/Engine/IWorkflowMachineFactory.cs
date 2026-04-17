using SyncWorkflow.Domain.Runtime;

namespace SyncWorkflow.Engine;

public interface IWorkflowMachineFactory : IFactory<WorkflowInstance, IWorkflowMachine>;