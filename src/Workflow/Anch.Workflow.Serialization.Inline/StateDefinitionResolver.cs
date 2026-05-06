using Anch.Workflow.Domain.Definition;

namespace Anch.Workflow.Serialization.Inline;

public class StateDefinitionResolver<TSource, TStatus>(IWorkflowDefinition<TSource, TStatus> workflow) : IStateDefinitionResolver<TSource>
    where TSource : notnull
    where TStatus : struct
{
    private readonly Func<TSource, TStatus> getStatus = (workflow.StatusAccessors ?? throw new InvalidOperationException("StatusAccessors cannot be null"))
        .Getter;

    //private readonly Func<TSource, TStatus> getStatus = ((Expression<Func<TSource, TStatus>>)workflow.Definition.DomainBindingInfo.StatusProperty!).Compile(LambdaCompileCache.Default);

    private readonly Dictionary<TStatus, IStateDefinition<TSource, TStatus>> statusMap = workflow.States.Where(state => state.Status != null).ToDictionary(st => st.Status!.Value);

    public IStateDefinition GetCurrentStateDefinition(TSource source)
    {
        return this.statusMap[this.getStatus(source)];

        //var stateDefinition = workflow.Definition.States.Single(state => (TStatus)state.Status == this.statusExpr. );

        //throw new NotImplementedException();

        //var isTerminated = workflowInfo.IsTerminated;
        //var isFinished = workflowInfo.IsFinished;

        //var isTerminatedOrFinished = isTerminated || isFinished;

        //var currentStateDefinition =

        //    isTerminated ? this.workflow.Definition.TerminateState
        //    : isFinished ? this.workflow.Definition.FinalState
        //    : this.workflow.Definition.States.Single(s => s.Name == source.Status.ToString());
    }
}