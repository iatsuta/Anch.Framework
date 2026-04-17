using System.Linq.Expressions;
using CommonFramework.ExpressionEvaluate;
using SyncWorkflow.Domain.Definition;

namespace SyncWorkflow.Storage.Inline;

public class StateDefinitionResolver<TSource, TStatus>(IWorkflow<TSource> workflow) : IStateDefinitionResolver<TSource>
    where TStatus : notnull
{
    private readonly Func<TSource, TStatus> getStatus = ((Expression<Func<TSource, TStatus>>)workflow.Definition.StatusProperty!).Compile(LambdaCompileCache.Default);

    private readonly Dictionary<TStatus, IStateDefinition> statusMap = workflow.Definition.States.Where(state => state.Status != null).ToDictionary(st => (TStatus)st.Status!);

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