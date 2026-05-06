using Anch.Workflow.Builder.Default.DomainDefinition;
using Anch.Workflow.Domain;
using Anch.Workflow.States;

namespace Anch.Workflow.Builder.Default;

public class TaskBuilder<TSource, TStatus>(WorkflowDefinitionBuilder<TSource, TStatus> workflowDefinitionBuilder, IStateBuilder<TSource, TStatus, TaskState> taskState) : ITaskBuilder<TSource, TStatus>
    where TSource : notnull
    where TStatus : struct
{
    private readonly List<EventHeader> commands = [];

    public IReadOnlyList<EventHeader> Commands => this.commands;

    public ITaskBuilder<TSource, TStatus> AddCommand(EventHeader eventHeader, Action<IWorkflowBuilder<TSource, TStatus>> branchSetup)
    {
        var innerDefinition = workflowDefinitionBuilder.CloneHeader();

        branchSetup(new WorkflowBuilder<TSource, TStatus>(innerDefinition));

        workflowDefinitionBuilder.Attach(taskState.StateDefinitionBuilder, eventHeader, innerDefinition);

        this.commands.Add(eventHeader);

        return this;
    }
}