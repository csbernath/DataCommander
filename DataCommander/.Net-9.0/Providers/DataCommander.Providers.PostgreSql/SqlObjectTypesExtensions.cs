using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql;

internal static class SqlObjectTypesExtensions
{
    public static List<string> ToTableTypes(this SqlObjectTypes sqlObjectTypes)
    {
        List<string> list = [];

        if (sqlObjectTypes.HasFlag(SqlObjectTypes.Table))
        {
            list.Add(TableType.BaseTable);
        }

        if (sqlObjectTypes.HasFlag(SqlObjectTypes.View))
        {
            list.Add(TableType.View);
        }

        //if (sqlObjectTypes.HasFlag(SqlObjectTypes.Function))
        //{
        //    list.Add(SqlServerObjectType.ScalarFunction);
        //    list.Add(SqlServerObjectType.InlineTableValuedFunction);
        //    list.Add(SqlServerObjectType.TableValuedFunction);
        //}

        return list;
    }

    public static List<string> ToObjectTypes(this SqlObjectTypes sqlObjectTypes)
    {
        List<string> list = [];

        if (sqlObjectTypes.HasFlag(SqlObjectTypes.Table))
        {
            list.Add(SqlServerObjectType.UserDefinedTable);
            list.Add(SqlServerObjectType.SystemTable);
        }

        if (sqlObjectTypes.HasFlag(SqlObjectTypes.View))
        {
            list.Add(SqlServerObjectType.View);
        }

        if (sqlObjectTypes.HasFlag(SqlObjectTypes.Function))
        {
            list.Add(SqlServerObjectType.ScalarFunction);
            list.Add(SqlServerObjectType.InlineTableValuedFunction);
            list.Add(SqlServerObjectType.TableValuedFunction);
        }

        return list;
    }
}