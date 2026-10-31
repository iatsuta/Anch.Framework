using Anch.GenericQueryable.Services;

using Microsoft.EntityFrameworkCore;

namespace Anch.GenericQueryable.EntityFramework;

/// <summary>Maps <c>GenericXxx</c> operations to the methods of <c>EntityFrameworkQueryableExtensions</c>.</summary>
public class EfTargetMethodExtractor() : TargetMethodExtractor([typeof(EntityFrameworkQueryableExtensions)]);