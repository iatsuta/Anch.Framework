using Xunit.v3;

namespace CommonFramework.Testing;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CommonMemberDataAttribute(string memberName, params object?[] arguments) : MemberDataAttributeBase(memberName, arguments);
