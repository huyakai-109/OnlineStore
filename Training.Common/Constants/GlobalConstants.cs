namespace Training.Common.Constants
{
    public static class GlobalConstants
    {
        public const string JWTLoginToken = "JWTLoginToken";

        public struct UserToken
        {
            public const string LoginProvider = "Self-hosted";
        }

        public struct Files
        {
            public const string CsvExtension = "csv";

            public const string CsvContentType = "text/csv";

            public const string ExcelExtension = "xlsx";

            public const string ExcelContentType = "application/excel";
        }

        public struct Culture
        {
            public const string English = "en-US";
            public const string Vietnam = "vi-VN";
        }

        public struct SortDirection
        {
            public const string Ascending = "asc";
            public const string Descending = "desc";
        }

        public struct StorageErrorMessage
        {
            public const string BucketIsNull = "Bucket is null.";
            public const string BucketNotFound = "Bucket not found.";
            public const string UploadFailed = "Upload file failed.";
        }

        public struct Symbol
        {
            public const string ForwardSlash = "/";
            public const string BackSlash = @"\";
            public const string UnderScore = "_";
            public const string Comma = ",";
            public const string Space = " ";
            public const string Dash = "-";
            public const string Question = "?";
            public const string Colon = ":";
            public const string LeftBracket = "[";
            public const string RightBracket = "]";
            public const string Asterisk = "*";
            public const string Dot = ".";
            public const string AtSign = "@";
            public const string SemiColon = ";";
            public const string Hash = "#";
        }
    }
}
