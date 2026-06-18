using System.Collections.Immutable;

using Anch.SecuritySystem.PermissionOptimization;

namespace Anch.SecuritySystem.DiTests;

public class RuntimePermissionOptimizationServiceTests
{
    private readonly RuntimePermissionOptimizationService service = new();

    [Theory]
    [MemberData(nameof(GetTestCasesData))]
    public void Optimize(TestCase testCase)
    {
        // Arrange
        var inputPermissions = testCase.Permissions.Select(p => p.Restrictions);
        var expectedPermission = (testCase.Expected ?? testCase.Permissions).Select(permission => permission.Squash().Restrictions);

        // Act
        var optimizedPermissions = this.service.Optimize(inputPermissions).ToList();

        // Assert
        Assert.Equal(expectedPermission, optimizedPermissions);
    }

    public static TheoryData<TestCase> GetTestCasesData()
    {
        return new(GetOldTestCases().Concat(GetNewTestCases()));
    }

    private static IEnumerable<TestCase> GetNewTestCases()
    {
        // --- 1 context ---

        {
            yield return new TestCase("C1_0",
                [
                    new TestPermission(new() { { typeof(TestSecurityContext1), Array.Empty<Guid>() } })
                ],
                [
                    TestPermission.Unrestricted
                ]);
        }

        {
            var id = Guid.NewGuid();

            yield return new TestCase("C1_1",
            [
                new TestPermission(new() { { typeof(TestSecurityContext1), new[] { id } } })
            ], null);
        }

        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            yield return new TestCase("C1_2",
                [
                    new TestPermission(new() { { typeof(TestSecurityContext1), new[] { id1 } } }),
                    new TestPermission(new() { { typeof(TestSecurityContext1), new[] { id1, id2 } } })
                ],
                [
                    new TestPermission(new() { { typeof(TestSecurityContext1), new[] { id1, id2 } } })
                ]);
        }

        {
            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            yield return new TestCase("C1_3",
                [
                    new TestPermission(new() { { typeof(TestSecurityContext1), Array.Empty<Guid>() } }),
                    new TestPermission(new() { { typeof(TestSecurityContext1), new[] { id1, id2 } } })
                ],
                [
                    TestPermission.Unrestricted
                ]);
        }

        // --- 2 contexts ---

        {
            // both contexts unrestricted → empty dict
            yield return new TestCase("C2_0",
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext2), Array.Empty<Guid>() }
                    })
                ],
                [
                    TestPermission.Unrestricted
                ]);
        }

        {
            // one unrestricted, one restricted → keeps only restricted context
            var id = Guid.NewGuid();

            yield return new TestCase("C2_1",
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext2), new[] { id } }
                    })
                ],
                [
                    new TestPermission(new() { { typeof(TestSecurityContext2), new[] { id } } })
                ]);
        }

        {
            // same C2, different C1 → merge C1
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var x = Guid.NewGuid();

            yield return new TestCase("C2_2",
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), new[] { a } },
                        { typeof(TestSecurityContext2), new[] { x } }
                    }),
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), new[] { b } },
                        { typeof(TestSecurityContext2), new[] { x } }
                    })
                ],
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), new[] { a, b } },
                        { typeof(TestSecurityContext2), new[] { x } }
                    })
                ]);
        }

        {
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();

            // disjoint contexts, nothing to merge → permissions stay separate (cf. L_3)
            yield return new TestCase("C2_3",
            [
                new TestPermission(new()
                {
                    { typeof(TestSecurityContext1), new[] { a } },
                }),
                new TestPermission(new()
                {
                    { typeof(TestSecurityContext2), new[] { b } }
                })
            ], null);
        }

        // --- 3 contexts ---

        {
            // all three contexts unrestricted → empty dict
            yield return new TestCase("C3_0",
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext2), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext3), Array.Empty<Guid>() }
                    })
                ],
                [
                    TestPermission.Unrestricted
                ]);
        }

        {
            // two unrestricted, one restricted → keeps only restricted context
            var id = Guid.NewGuid();

            yield return new TestCase("C3_1",
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext2), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext3), new[] { id } }
                    })
                ],
                [
                    new TestPermission(new() { { typeof(TestSecurityContext3), new[] { id } } })
                ]);
        }

        {
            // same C2+C3, different C1 → merge C1
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var x = Guid.NewGuid();
            var p = Guid.NewGuid();

            yield return new TestCase("C3_3",
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), new[] { a } },
                        { typeof(TestSecurityContext2), new[] { x } },
                        { typeof(TestSecurityContext3), new[] { p } }
                    }),
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), new[] { b } },
                        { typeof(TestSecurityContext2), new[] { x } },
                        { typeof(TestSecurityContext3), new[] { p } }
                    })
                ],
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), new[] { a, b } },
                        { typeof(TestSecurityContext2), new[] { x } },
                        { typeof(TestSecurityContext3), new[] { p } }
                    })
                ]);
        }

        {
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();

            yield return new TestCase("C3_3",
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext2), new[] { a } },
                        { typeof(TestSecurityContext3), Array.Empty<Guid>() },
                    }),
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext2), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext3), new[] { b } },
                    })
                ],
                null);
        }
    }


    private static IEnumerable<TestCase> GetOldTestCases()
    {
        {
            var testId = Guid.NewGuid();

            // {C1:[]} = no restriction on C1 = full access → absorbs the restricted permission
            yield return new TestCase("L_0",
                [
                    new TestPermission(new() { { typeof(TestSecurityContext1), Array.Empty<Guid>() } }),
                    new TestPermission(new() { { typeof(TestSecurityContext1), new[] { testId } } })
                ],
                [
                    TestPermission.Unrestricted
                ]);
        }

        {
            var testId = Guid.NewGuid();

            // first permission has no real restrictions ({C1:[],C2:[]}) = full access → absorbs all
            yield return new TestCase("L_1",
                [
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext2), Array.Empty<Guid>() }
                    }),
                    new TestPermission(new()
                    {
                        { typeof(TestSecurityContext1), Array.Empty<Guid>() },
                        { typeof(TestSecurityContext2), new[] { testId } }
                    })
                ],
                [
                    TestPermission.Unrestricted
                ]);
        }

        {
            var g1 = Guid.NewGuid();
            var g2 = Guid.NewGuid();

            yield return new TestCase("L_2",
                [
                    new TestPermission(new() { { typeof(string), new[] { g1 } } }),
                    new TestPermission(new() { { typeof(string), new[] { g2 } } }),
                    new TestPermission(new() { { typeof(string), new[] { g1 } } })
                ],
                [
                    new TestPermission(new() { { typeof(string), new[] { g1, g2 } } })
                ]);
        }


        {
            var g3 = Guid.NewGuid();

            yield return new TestCase("L_3",
                [
                    new TestPermission(new() { { typeof(string), new[] { g3 } } }),
                    new TestPermission(new() { { typeof(int), new[] { 42 } } })
                ],
                null);
        }

        {
            var g1 = Guid.NewGuid();
            var g2 = Guid.NewGuid();

            yield return new TestCase("L_4",
                [
                    new TestPermission(new() { { typeof(string), new[] { g1 } } }),
                    new TestPermission(new() { { typeof(string), new[] { g2 } } }),
                    new TestPermission(new()
                    {
                        { typeof(string), new[] { g1 } },
                        { typeof(int), new[] { 1, 2, 3 } }
                    })
                ],
                [
                    new TestPermission(new()
                    {
                        { typeof(string), new[] { g1 } },
                        { typeof(int), new[] { 1, 2, 3 } }
                    }),
                    new TestPermission(new() { { typeof(string), new[] { g2 } } })
                ]);
        }

        {
            yield return new TestCase("L_5", [], []);
        }

        {
            yield return new TestCase("L_6",
                [
                    new TestPermission(new() { { typeof(int), new[] { 1, 2 } } }),
                    new TestPermission(new() { { typeof(int), new[] { 2, 3 } } })
                ],
                [
                    new TestPermission(new() { { typeof(int), new[] { 1, 2, 3 } } })
                ]);
        }
    }



    private class TestSecurityContext1 : ISecurityContext;

    private class TestSecurityContext2 : ISecurityContext;

    private class TestSecurityContext3 : ISecurityContext;

    public record TestCase(string Name, ImmutableArray<TestPermission> Permissions, ImmutableArray<TestPermission>? Expected)
    {
        public override string ToString() => this.Name;
    }

    public record TestPermission(Dictionary<Type, Array> Restrictions)
    {
        public TestPermission Squash()
        {
            return new TestPermission(this.Restrictions.Where(pair => pair.Value.Length > 0).ToDictionary());
        }

        public static TestPermission Unrestricted { get; } = new([]);
    }
}