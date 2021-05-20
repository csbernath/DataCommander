﻿using Foundation.Assertions;
using Foundation.Core;
using Foundation.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace DataCommander.Providers.SqlServer
{
    internal static class SqlServerObject
    {
        public static string GetDatabases() => "select name from sys.databases (nolock) order by name";
        public static string GetSchemas() => "select name from sys.schemas (nolock) order by name";

        public static string GetSchemas(string database)
        {
            Assert.IsTrue(!database.IsNullOrWhiteSpace());

            return $@"if exists(select * from sys.databases (nolock) where name = '{database}')
begin
    select null,name
    from [{database}].sys.schemas (nolock)
    order by name
end";
        }

        public static string GetObjects(string schema, IEnumerable<string> objectTypes)
        {
            Assert.IsTrue(!schema.IsNullOrWhiteSpace());
            Assert.IsNotNull(objectTypes);
            Assert.IsTrue(objectTypes.Any());

            return $@"declare @schema_id int

select @schema_id = schema_id
from sys.schemas (nolock)
where name = '{schema}'

if @schema_id is not null
begin
    select o.name
    from sys.all_objects o (nolock)
    where
        o.schema_id = @schema_id
        and o.type in({string.Join(",", objectTypes.Select(o => o.ToNullableVarChar()))})
    order by o.name
end";
        }

        public static string GetObjects(string database, string schema, IEnumerable<string> objectTypes)
        {
            Assert.IsTrue(!database.IsNullOrWhiteSpace());
            Assert.IsTrue(!schema.IsNullOrWhiteSpace());
            Assert.IsTrue(objectTypes != null && objectTypes.Any());

            return $@"if exists(select * from sys.databases (nolock) where name = '{database}')
begin
    declare @schema_id int

    select @schema_id = schema_id
    from [{database}].sys.schemas (nolock)
    where name = '{schema}'

    if @schema_id is not null
    begin
        select o.name
        from [{database}].sys.all_objects o (nolock)
        where
            o.schema_id = @schema_id
            and o.type in({string.Join(",", objectTypes.Select(t => t.ToNullableVarChar()))})
        order by o.name
    end
end";
        }
    }
}