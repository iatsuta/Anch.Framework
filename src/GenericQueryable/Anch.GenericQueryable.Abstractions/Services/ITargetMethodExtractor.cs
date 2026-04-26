using System.Reflection;

namespace Anch.GenericQueryable.Services;

public interface ITargetMethodExtractor
{
	MethodInfo? TryGetTargetMethod(MethodInfo baseMethod);
}