namespace Training.Common.Constants
{
    public static class ConfigKeys
    {
        public const string DatabaseConnection = "ConnectionStrings:MyDatabase";
        public const string EnableSwagger = "EnableSwagger";
        public const string AutoMigration = "AutoMigration";

        public struct Security
        {
            public struct Lockout
            {
                public const string MaxFailedAccessAttempts = "Security:Lockout:MaxFailedAccessAttempts";
                public const string DefaultLockoutMinutes = "Security:Lockout:DefaultLockoutMinutes";
            }

            public struct Jwt
            {
                public const string Secret = "Security:Jwt:Secret";
                public const string RefreshSecret = "Security:Jwt:RefreshSecret";
                public const string ExpirationMinutes = "Security:Jwt:ExpirationMinutes";
                public const string RefreshExpirationDays = "Security:Jwt:RefreshExpirationDays";
                public const string Issuer = "Security:Jwt:Issuer";
                public const string Audience = "Security:Jwt:Audience";
            }
        }
    }
}
