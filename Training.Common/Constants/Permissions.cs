namespace Training.Common.Constants
{
    public class Permissions
    {
        public static string[] All =
        [
            User.ViewUsers,
            User.ManageUsers,
            Products.ViewProducts,
            Products.ManageProducts,
            Categories.ViewCategories,
            Categories.ManageCategories,
            Stocks.ViewStocks,
            Stocks.ManageStocks,
            StockEvents.ViewStockEvents,
            Orders.ViewOrders,
            Reports.ViewReports
        ];

        /// <summary>
        /// Manage = Create, Update, Delete
        /// </summary>
        /// 
        public struct User
        {
            public const string ViewUsers = "ViewUsers";
            public const string ManageUsers = "ManageUsers";
        }

        public struct Products
        {
            public const string ViewProducts = "ViewProducts";
            public const string ManageProducts = "ManageProducts";
        }

        public struct Categories
        {
            public const string ViewCategories = "ViewCategories";
            public const string ManageCategories = "ManageCategories";
        }

        public struct Stocks
        {
            public const string ViewStocks = "ViewStocks";
            public const string ManageStocks = "ManageStocks";
        }

        public struct StockEvents
        {
            public const string ViewStockEvents = "ViewStockEvents";
        }

        public struct Orders
        {
            public const string ViewOrders = "ViewOrders";
        }

        public struct Reports
        {
            public const string ViewReports = "ViewReports";
        }
    }
}
