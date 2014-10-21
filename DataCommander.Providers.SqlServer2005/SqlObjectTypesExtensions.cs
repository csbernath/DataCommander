namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;

    internal static class SqlObjectTypesExtensions
    {
        public static List<string> ToObjectTypes(this SqlObjectTypes sqlObjectTypes)
        {
            var list = new List<string>();

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
}