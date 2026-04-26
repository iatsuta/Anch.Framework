using System.Reflection;

[assembly: AssemblyProduct("ExampleApp")]
[assembly: AssemblyCompany("Anch")]

[assembly: AssemblyInformationalVersion("changes at build")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif