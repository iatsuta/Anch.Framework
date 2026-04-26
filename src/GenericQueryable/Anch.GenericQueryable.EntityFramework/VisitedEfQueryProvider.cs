using Anch.GenericQueryable.Services;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Anch.GenericQueryable.EntityFramework;

public class VisitedEfQueryProvider(IQueryCompiler queryCompiler, IGenericQueryableExecutor executor)
    : EntityQueryProvider(queryCompiler), IGenericQueryProvider

{
    public IGenericQueryableExecutor Executor { get; } = executor;
}