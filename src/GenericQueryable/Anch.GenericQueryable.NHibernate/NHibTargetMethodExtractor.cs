using Anch.GenericQueryable.Services;
using NHibernate.Linq;

namespace Anch.GenericQueryable.NHibernate;

public class NHibTargetMethodExtractor() : TargetMethodExtractor([typeof(LinqExtensionMethods), typeof(NHibLinqExtensions)]);