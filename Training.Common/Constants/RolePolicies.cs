namespace Training.Common.Constants
{
    public static class RolePolicies
    {
        public const string ClaimType = "RolePolicy";

        public static class SysAdmin
        {
            public const long Id = 1;
            public const string Name = "Admin";
            public const string DisplayName = "System Admin";

            public static string[] AllowedPermissions = Permissions.All;
        }
        public static class Clerk
        {
            public const long Id = 2;
            public const string Name = "Clerk";
            public const string DisplayName = "Clerk";

            public static string[] AllowedPermissions = Permissions.Clerk;
        }
    }
}
