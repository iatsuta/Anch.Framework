using Anch.Testing.XunitEngine;
using Xunit;
using Xunit.v3;

namespace Anch.Testing;

[XunitTestCaseDiscoverer(typeof(AnchFactDiscoverer))]
[AttributeUsage(AttributeTargets.Method)]
public class AnchFactAttribute : FactAttribute;