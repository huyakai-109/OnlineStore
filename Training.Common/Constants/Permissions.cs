namespace Training.Common.Constants
{
    public class Permissions
    {
        public static string[] All =
        [
            Employees.ViewEmployees,
            Employees.ManageEmployees,
            Customers.ViewCustomers,
            Customers.ManageCustomers,

            Products.ViewProducts,
            Products.ManageProducts,
            Categories.ViewCategories,
            Categories.ManageCategories,
            Stocks.ViewStocks,
            Stocks.ManageStocks,
            Orders.ViewOrders,
            Orders.ManageOrders,
            Reports.ViewReports

        ];

        /// <summary>
        /// Manage = Create, Edit, Update
        /// </summary>
        /// 
        public static string[] Clerk =
        {
            Products.ViewProducts,
            Products.ManageProducts,
            Categories.ViewCategories,
            Categories.ManageCategories,
            Stocks.ViewStocks,
            Stocks.ManageStocks,
            Orders.ViewOrders,
            Orders.ManageOrders,
            Reports.ViewReports
        };

        public struct Employees
        {
            public const string ViewEmployees = "ViewEmployees";

            public const string ManageEmployees = "ManageEmployees";
        }

        public struct Customers
        {
            public const string ViewCustomers = "ViewCustomers";

            public const string ManageCustomers = "ManageCustomers";
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

        public struct Orders
        {
            public const string ViewOrders = "ViewOrders";
            public const string ManageOrders = "ManageOrders";
        }

        public struct Reports
        {
            public const string ViewReports = "ViewReports";
        }
    }
}
