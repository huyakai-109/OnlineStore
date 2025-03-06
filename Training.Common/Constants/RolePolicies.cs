namespace Training.Common.Constants
{
    public static class RolePolicies
    {
        public const string ClaimType = "RolePolicy";

        public static class Admin
        {
            public const long Id = 1;
            public const string Name = "Admin";
            public const string DisplayName = "System Admin";

            public static string[] AllowedPermissions =
            [
                Permissions.User.ViewUsers,
                Permissions.User.ManageUsers,
                Permissions.Categories.ViewCategories,
                Permissions.Categories.ManageCategories,
                Permissions.Products.ViewProducts,
                Permissions.Products.ManageProducts,
                Permissions.Stocks.ViewStocks,
                Permissions.Stocks.ManageStocks,
                Permissions.StockEvents.ViewStockEvents,
                Permissions.Orders.ViewOrders,
                Permissions.Reports.ViewReports,
            ];
        }

        public static class Clerk
        {
            public const long Id = 2;
            public const string Name = "Clerk";
            public const string DisplayName = "Clerk";

            public static string[] AllowedPermissions =
            [
                Permissions.Categories.ViewCategories,
                Permissions.Categories.ManageCategories,
                Permissions.Products.ViewProducts,
                Permissions.Products.ManageProducts,
                Permissions.Stocks.ViewStocks,
                Permissions.StockEvents.ViewStockEvents,
                Permissions.Orders.ViewOrders,
                Permissions.Reports.ViewReports,
            ];
        }

        public static class Customer
        {
            public const long Id = 3;
            public const string Name = "Customer";
            public const string DisplayName = "Customer";
            public static string[] AllowedPermissions =[];
        }
    }
}
