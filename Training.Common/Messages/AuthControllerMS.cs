namespace Tricor.BillingProcess.Common.Messages
{
    public static class AuthControllerMS
    {
        public static class Login
        {
            public const string InvalidCredential = "Auth.Login.InvalidCredential";
            public const string WillBeLockedOut = "Auth.Login.WillBeLockedOut";
            public const string LockedOut = "Auth.Login.LockedOut";
            public const string InActive = "Auth.Login.InActive";
            public const string Exception = "Auth.Login.Exception";
        }

        public static class RefreshToken
        {
            public const string RequiredToken = "Auth.RefreshToken.RequiredToken";
            public const string Failed = "Auth.RefreshToken.Failed";
            public const string Exception = "Auth.RefreshToken.Exception";
        }

        public static class Logout
        {
            public const string RequiredToken = "Auth.Logout.RequiredToken";
        }
    }
}
