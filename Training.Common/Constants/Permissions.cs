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
        ];

        /// <summary>
        /// Manage = Create, Edit, Update
        /// </summary>

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
    }
}
