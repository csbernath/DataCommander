using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer;

internal static class SqlObjectTypesExtensions
{
    extension(SqlObjectTypes sqlObjectTypes)
    {
        public List<string> ToObjectTypes()
        {
            List<string> list = [];

            if (sqlObjectTypes.HasFlag(SqlObjectTypes.Table))
            {
                list.Add(SqlServerObjectType.UserDefinedTable);
                list.Add(SqlServerObjectType.SystemTable);
            }

            if (sqlObjectTypes.HasFlag(SqlObjectTypes.View))
                list.Add(SqlServerObjectType.View);

            if (sqlObjectTypes.HasFlag(SqlObjectTypes.Function))
            {
                list.Add(SqlServerObjectType.ScalarFunction);
                list.Add(SqlServerObjectType.InlineTableValuedFunction);
                list.Add(SqlServerObjectType.TableValuedFunction);
            }

            return list;
        }
    }
}