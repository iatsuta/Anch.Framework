using SyncWorkflow.Builder.Default.DomainDefinition;

using SyncWorkflow.States;

namespace SyncWorkflow.Builder.Default;

public class TaskBuilder<TSource>(WorkflowDefinition workflowDefinition, IStateBuilder<TSource, TaskState> taskState) : ITaskBuilder<TSource>
    where TSource : notnull
{
    private readonly List<EventHeader> commands = [];

    public IReadOnlyList<EventHeader> Commands => this.commands;

    public ITaskBuilder<TSource> AddCommand(EventHeader eventHeader, Action<IWorkflowBuilder<TSource>> branchSetup)
    {
        var innerDefinition = workflowDefinition.HeaderClone();

        branchSetup(new WorkflowBuilder<TSource>(innerDefinition));

        workflowDefinition.Attach(taskState.StateDefinition, eventHeader, innerDefinition);

        this.commands.Add(eventHeader);

        return this;
    }
}