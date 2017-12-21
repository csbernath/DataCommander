using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using Foundation.Data.SqlClient;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.SqlServer
{
    internal static class SqlServerObject
    {
        public static string GetDatabases()
        {
            return "select name from sys.databases (nolock) order by name";
        }

        public static string GetSchemas()
        {
            return "select name from sys.schemas (nolock) order by name";
        }

        public static string GetSchemas(string database)
        {
            FoundationContract.Requires<ArgumentOutOfRangeException>(!database.IsNullOrWhiteSpace());

            return string.Format(@"if exists(select * from sys.databases (nolock) where name = '{0}')
begin
    select null,name
    from [{0}].sys.schemas (nolock)
    order by name
end", database);
        }

        public static string GetObjects(string schema, IEnumerable<string> objectTypes)
        {
            FoundationContract.Requires<ArgumentException>(!schema.IsNullOrWhiteSpace());
            FoundationContract.Requires<ArgumentException>(objectTypes != null && objectTypes.Any());

            return
                $@"declare @schema_id int

select @schema_id = schema_id
from sys.schemas (nolock)
where name = '{schema
                    }'

if @schema_id is not null
begin
    select o.name
    from sys.all_objects o (nolock)
    where
        o.schema_id = @schema_id
        and o.type in({
                    string.Join(",", objectTypes.Select(o => o.ToTSqlVarChar()))})
    order by o.name
end";
        }

        public static string GetObjects(
            string database,
            string schema,
            IEnumerable<string> objectTypes)
        {
            FoundationContract.Requires<ArgumentException>(!database.IsNullOrWhiteSpace());
            FoundationContract.Requires<ArgumentException>(!schema.IsNullOrWhiteSpace());
            FoundationContract.Requires<ArgumentException>(objectTypes!=null && objectTypes.Any());

            return string.Format(@"if exists(select * from sys.databases (nolock) where name = '{0}')
begin
    declare @schema_id int

    select @schema_id = schema_id
    from [{0}].sys.schemas (nolock)
    where name = '{1}'

    if @schema_id is not null
    begin
        select o.name
        from [{0}].sys.all_objects o (nolock)
        where
            o.schema_id = @schema_id
            and o.type in({2})
        order by o.name
    end
end", database, schema, string.Join(",", objectTypes.Select(t => t.ToTSqlVarChar())));
        }
    }
}