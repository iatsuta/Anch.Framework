using Anch.Core;
using Anch.GenericRepository;
using Anch.HierarchicalExpand.Denormalization;
using Anch.HierarchicalExpand.IntegrationTests.Domain;

namespace Anch.HierarchicalExpand.IntegrationTests.Environment;

public class TestDataInitializer(ScopeEvaluator scopeEvaluator) : IInitializer
{
    public async Task Initialize(CancellationToken ct)
    {
        await scopeEvaluator.EvaluateAsync<IGenericRepository>(async genericRepository =>
        {
            foreach (var bu in GetTestBusinessUnits())
            {
                await genericRepository.SaveAsync(bu, ct);
            }
        });

        await scopeEvaluator.EvaluateAsync<IAncestorDenormalizer>(ancestorDenormalizer => ancestorDenormalizer.Initialize(ct));
    }

    private static IEnumerable<BusinessUnit> GetTestBusinessUnits()
    {
        var rootBu = new BusinessUnit { Name = "TestRootBu" };
        yield return rootBu;

        foreach (var index in Enumerable.Range(1, 2))
        {
            var middleBu = new BusinessUnit { Name = $"Test{nameof(BusinessUnit)}{index}", Parent = rootBu };
            yield return middleBu;

            var leafBu = new BusinessUnit { Name = $"Test{nameof(BusinessUnit)}{index}-Child", Parent = middleBu };
            yield return leafBu;
        }
    }
}