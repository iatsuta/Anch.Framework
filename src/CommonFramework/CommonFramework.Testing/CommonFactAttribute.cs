using CommonFramework.Testing.XunitEngine;

using Xunit;
using Xunit.v3;

namespace CommonFramework.Testing;

[XunitTestCaseDiscoverer(typeof(CommonFactDiscoverer))]
[AttributeUsage(AttributeTargets.Method)]
public class CommonFactAttribute : FactAttribute;