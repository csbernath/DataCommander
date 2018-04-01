using System;
using Foundation.Data.SqlClient;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.MySql
{
    using System.Collections.Generic;
    using System.Linq;
    using Foundation;

    internal static class SqlServerObject
    {
        public static string GetDatabases()
        {
            return @"select SCHEMA_NAME from information_schema.SCHEMATA order by SCHEMA_NAME";
        }

        public static string GetTables(string tableSchema, IEnumerable<string> tableTypes)
        {
            FoundationContract.Requires<ArgumentException>(!tableSchema.IsNullOrWhiteSpace());
            FoundationContract.Requires<ArgumentException>(tableTypes != null && tableTypes.Any());
            
            return $@"select TABLE_NAME
from information_schema.TABLES
where
    TABLE_SCHEMA = {tableSchema.ToTSqlVarChar()}
    and TABLE_TYPE in({string.Join(",", tableTypes.Select(o => o.ToTSqlVarChar()))})
order by TABLE_NAME";
        }

        public static string GetColumns(string tableSchema, string tableName)
        {
            return $@"select COLUMN_NAME
from information_schema.COLUMNS
where
    TABLE_SCHEMA = {tableSchema.ToTSqlVarChar()}
    and TABLE_NAME = {tableName.ToTSqlVarChar()}
order by ORDINAL_POSITION";
        }
    }
}