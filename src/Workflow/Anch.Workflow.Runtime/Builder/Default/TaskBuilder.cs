using Anch.Workflow.Builder.Default.DomainDefinition;
using Anch.Workflow.Domain;
using Anch.Workflow.States;

namespace Anch.Workflow.Builder.Default;

public class TaskBuilder<TSource, TStatus>(WorkflowDefinitionBuilder<TSource, TStatus> workflowDefinition, IStateBuilder<TSource, TStatus, TaskState> taskState) : ITaskBuilder<TSource, TStatus>
    where TSource : notnull
    where TStatus : notnull
{
    private readonly List<EventHeader> commands = [];

    public IReadOnlyList<EventHeader> Commands => this.commands;

    public ITaskBuilder<TSource, TStatus> AddCommand(EventHeader eventHeader, Action<IWorkflowBuilder<TSource, TStatus>> branchSetup)
    {
        var innerDefinition = workflowDefinition.CloneHeader();

        branchSetup(new WorkflowBuilder<TSource, TStatus>(innerDefinition));

        workflowDefinition.Attach(taskState.StateDefinitionBuilder, eventHeader, innerDefinition);

        this.commands.Add(eventHeader);

        return this;
    }
}