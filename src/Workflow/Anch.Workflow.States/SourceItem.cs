namespace Anch.Workflow.States;

public record SourceItem<TSource, TElement>(TSource Source, TElement Element);