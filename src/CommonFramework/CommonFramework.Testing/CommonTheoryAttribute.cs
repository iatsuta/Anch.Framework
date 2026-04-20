using Xunit;
using Xunit.v3;

namespace CommonFramework.Testing;

[XunitTestCaseDiscoverer(typeof(CommonTheoryDiscoverer))]
[AttributeUsage(AttributeTargets.Method)]
public class CommonTheoryAttribute : TheoryAttribute;