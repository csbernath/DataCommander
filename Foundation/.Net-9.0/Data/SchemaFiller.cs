using System;
using System.Collections.Generic;
using System.Data;

namespace Foundation.Data;

public static class SchemaFiller
{
    public static DataTable FillSchema(IDataReader dataReader, DataTable dataTable)
    {
        var schemaTable = dataReader.GetSchemaTable();
        FillSchema(schemaTable, dataTable);
        return schemaTable;
    }

    internal static void FillSchema(DataTable schemaTable, DataTable dataTable)
    {
        List<DataColumn> primaryKey = [];
        var columns = dataTable.Columns;
        var isKeyColumn = columns["IsKey"];

        foreach (DataRow row in schemaTable.Rows)
        {
            var columnName = (string)row["ColumnName"];
            var dataType = (Type)row["DataType"];
            var isKey = isKeyColumn != null && row.GetNullableValueField<bool>(isKeyColumn) == true;
            var columnNameAdd = columnName;
            var index = 2;

            while (true)
            {
                if (columns.Contains(columnNameAdd))
                {
                    columnNameAdd = $"{columnName}{index}";
                    ++index;
                }
                else
                    break;
            }

            var column = new DataColumn(columnNameAdd, dataType);
            columns.Add(column);

            if (isKey)
                primaryKey.Add(column);
        }

        var array = primaryKey.ToArray();
        dataTable.PrimaryKey = array;
    }
}