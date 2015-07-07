namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Foundation.Data.SqlClient;

    internal static class SqlServerObject
    {
        public static string GetDatabases()
        {
            return @"select SCHEMA_NAME from information_schema.SCHEMATA order by SCHEMA_NAME";
        }

        public static string GetTables(string tableSchema, IEnumerable<string> tableTypes)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(tableSchema));
            Contract.Requires(tableTypes != null && tableTypes.Any());

            return string.Format(@"select TABLE_NAME
from information_schema.TABLES
where
    TABLE_SCHEMA = {0}
    and TABLE_TYPE in({1})
order by TABLE_NAME", tableSchema.ToTSqlVarChar(), string.Join(",", tableTypes.Select(o => o.ToTSqlVarChar())));
        }

        public static string GetColumns(string tableSchema, string tableName)
        {
            return string.Format(@"select COLUMN_NAME
from information_schema.COLUMNS
where
    TABLE_SCHEMA = {0}
    and TABLE_NAME = {1}
order by ORDINAL_POSITION", tableSchema.ToTSqlVarChar(), tableName.ToTSqlVarChar());
        }
    }
}