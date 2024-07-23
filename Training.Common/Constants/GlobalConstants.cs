namespace Training.Common.Constants
{
    public static class GlobalConstants
    {
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
    }
}
