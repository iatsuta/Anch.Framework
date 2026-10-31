using Anch.Testing.Xunit.Engine;

using Xunit;
using Xunit.v3;

namespace Anch.Testing.Xunit;

/// <summary>Marks a test method run by the Anch test framework. The test class receives its services through the constructor and the method may take parameters resolved from the container.</summary>
[XunitTestCaseDiscoverer(typeof(AnchFactDiscoverer))]
[AttributeUsage(AttributeTargets.Method)]
public class AnchFactAttribute : FactAttribute;