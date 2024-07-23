namespace Training.Common.Constants
{
    public static class RolePolicies
    {
        public const string ClaimType = "RolePolicy";

        public static class SysAdmin
        {
            public const long Id = 1;
            public const string Name = "SysAdmin";
            public const string DisplayName = "System Admin";

            public static string[] AllowedPermissions = Permissions.All;
        }
    }
}
