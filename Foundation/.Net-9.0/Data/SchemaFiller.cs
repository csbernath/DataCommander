using System;
using System.Collections.Generic;
using System.Data;

namespace Foundation.Data;

public static class SchemaFiller
{
    public static DataTable FillSchema(IDataReader dataReader, DataTable dataTable)
    {
        DataTable schemaTable = dataReader.GetSchemaTable();
        FillSchema(schemaTable, dataTable);
        return schemaTable;
    }

    internal static void FillSchema(DataTable schemaTable, DataTable dataTable)
    {
        List<DataColumn> primaryKey = [];
        DataColumnCollection columns = dataTable.Columns;
        DataColumn isKeyColumn = columns["IsKey"];

        foreach (DataRow row in schemaTable.Rows)
        {
            string columnName = (string)row["ColumnName"];
            Type dataType = (Type)row["DataType"];
            bool isKey = isKeyColumn != null && row.GetNullableValueField<bool>(isKeyColumn) == true;
            string columnNameAdd = columnName;
            int index = 2;

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

            DataColumn column = new DataColumn(columnNameAdd, dataType);
            columns.Add(column);

            if (isKey)
                primaryKey.Add(column);
        }

        DataColumn[] array = primaryKey.ToArray();
        dataTable.PrimaryKey = array;
    }
}