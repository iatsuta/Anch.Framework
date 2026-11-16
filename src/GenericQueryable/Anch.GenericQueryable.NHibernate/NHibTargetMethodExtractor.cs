using Anch.GenericQueryable.Services;

using NHibernate.Linq;

namespace Anch.GenericQueryable.NHibernate;

/// <summary>Maps <c>GenericXxx</c> operations to the NHibernate LINQ extension methods.</summary>
public class NHibTargetMethodExtractor() : TargetMethodExtractor([typeof(LinqExtensionMethods), typeof(NHibLinqExtensions)]);