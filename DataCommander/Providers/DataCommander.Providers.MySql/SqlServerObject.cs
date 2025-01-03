using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace DataCommander.Providers.MySql;

internal static class SqlServerObject
{
    public static string GetDatabases()
    {
        return @"select SCHEMA_NAME from information_schema.SCHEMATA order by SCHEMA_NAME";
    }

    public static string GetTables(string tableSchema, IEnumerable<string> tableTypes)
    {
        Assert.IsTrue(!tableSchema.IsNullOrWhiteSpace());
        Assert.IsTrue(tableTypes != null && tableTypes.Any());

        return $@"select TABLE_NAME
from information_schema.TABLES
where
    TABLE_SCHEMA = {tableSchema.ToNullableVarChar()}
    and TABLE_TYPE in({string.Join(",", tableTypes.Select(o => o.ToNullableVarChar()))})
order by TABLE_NAME";
    }

    public static string GetColumns(string tableSchema, string tableName)
    {
        return $@"select COLUMN_NAME
from information_schema.COLUMNS
where
    TABLE_SCHEMA = {tableSchema.ToNullableVarChar()}
    and TABLE_NAME = {tableName.ToNullableVarChar()}
order by ORDINAL_POSITION";
    }
}