namespace DataCommander.Providers.SqlServer
{
    internal static class SqlServerObjectType
    {
        // FN = SQL scalar function
        public const string ScalarFunction = "FN";

        //IF = SQL inline table-valued function
        public const string InlineTableValuedFunction = "TF";

        // TF = SQL table-valued-function
        public const string TableValuedFunction = "TF";

        // U = Table (user-defined)
        public const string UserDefinedTable = "U";

        // S = System base table
        public const string SystemTable = "S";

        // V = View
        public const string View = "V";
    }
}