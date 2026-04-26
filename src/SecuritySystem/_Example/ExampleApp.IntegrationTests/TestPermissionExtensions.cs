using Anch.SecuritySystem;
using Anch.SecuritySystem.Testing;
using ExampleApp.Domain;

namespace ExampleApp.IntegrationTests;

public static class TestPermissionExtensions
{
    public const string ExtendedKey = nameof(Domain.Auth.General.Permission.ExtendedValue);

    extension(TestPermission testPermission)
    {
        public TypedSecurityIdentity<Guid>? BusinessUnit
        {
            get => testPermission.GetSingle<BusinessUnit, Guid>();
            set => testPermission.SetSingle<BusinessUnit, Guid>(value);
        }

        public TypedSecurityIdentity<Guid>? ManagementUnit
        {
            get => testPermission.GetSingle<ManagementUnit, Guid>();
            set => testPermission.SetSingle<ManagementUnit, Guid>(value);
        }

        public TypedSecurityIdentity<Guid>? Employee
        {
            get => testPermission.GetSingle<Employee, Guid>();
            set => testPermission.SetSingle<Employee, Guid>(value);
        }

        public TypedSecurityIdentity<Guid>[] BusinessUnits
        {
            get => testPermission.GetMany<BusinessUnit, Guid>();
            set => testPermission.SetMany<BusinessUnit, Guid>(value);
        }

        public string ExtendedValue
        {
            get => (string)testPermission.ExtendedData[ExtendedKey];
            set => testPermission.ExtendedData[ExtendedKey] = value;
        }
    }
}