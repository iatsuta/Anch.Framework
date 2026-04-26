using Anch.GenericQueryable.Services;
using Microsoft.EntityFrameworkCore;

namespace Anch.GenericQueryable.EntityFramework;

public class EfTargetMethodExtractor() : TargetMethodExtractor([typeof(EntityFrameworkQueryableExtensions)]);