using System;
using System.Data;
using System.Data.OleDb;
using ADODB;

namespace DataCommander.Application;

public static class OleDbHelper
{
    public static DataTable Convert(object adodbRecordset)
    {
        var adapter = new OleDbDataAdapter();
        var dataTable = new DataTable();
        adapter.Fill(dataTable, adodbRecordset);
        return dataTable;
    }

    [CLSCompliant(false)]
    public static DataTable Convert(_Recordset recordset, out OleDbParameter[] columns)
    {
        var adapter = new OleDbDataAdapter();
        var dataTable = new DataTable();
        adapter.Fill(dataTable, recordset);
        columns = new OleDbParameter[recordset.Fields.Count];
        var index = 0;

        foreach (Field field in recordset.Fields)
        {
            var param = new OleDbParameter();
            param.SourceColumn = field.Name;
            param.OleDbType = (OleDbType)field.Type;

            var size = field.DefinedSize;
            var precision = field.Precision;

            if (size == 0) 
                size = precision;

            param.Size = size;
            param.Precision = precision;
            param.Scale = field.NumericScale;
            param.IsNullable = (field.Attributes & (int)FieldAttributeEnum.adFldIsNullable) != 0;

            columns[index] = param;

            index++;
        }

        return dataTable;
    }

    public static void DropTable(string tableName, OleDbConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = "drop table " + tableName;

        try
        {
            command.ExecuteNonQuery();
        }
        catch
        {
        }
    }

    private static void CreateTableSql(string tableName, OleDbParameter[] columns, OleDbConnection connection)
    {
        var cmdText = "create table " + tableName + "(";
        var i = 0;
        var count = columns.Length;

        foreach (var column in columns)
        {
            string sqlType;

            switch (column.OleDbType)
            {
                case OleDbType.Char:
                    sqlType = "char(" + column.Size + ")";
                    break;

                case OleDbType.DBDate:
                    sqlType = "datetime";
                    break;

                case OleDbType.DBTimeStamp:
                    //sqlType = "timestamp";
                    sqlType = "datetime";
                    break;

                case OleDbType.UnsignedTinyInt:
                    sqlType = "tinyint";
                    break;

                case OleDbType.Integer:
                    sqlType = "int";
                    break;

                case OleDbType.Numeric:
                    sqlType = "numeric(" + column.Precision + "," + column.Scale + ")";
                    break;

                case OleDbType.Decimal:
                    sqlType = "decimal(" + column.Precision + "," + column.Scale + ")";
                    break;

                case OleDbType.VarChar:
                    sqlType = "varchar(" + column.Size + ")";
                    break;

                default:
                    sqlType = "varchar(255)";
                    break;
            }

            cmdText += column.SourceColumn + " " + sqlType;

            if (column.IsNullable)
                cmdText += " NULL";
            else
                cmdText += " NOT NULL";

            if (i < count - 1) 
                cmdText += ",";

            cmdText += Environment.NewLine;

            i++;
        }

        cmdText += ")";

        var command = connection.CreateCommand();
        command.CommandText = cmdText;
        command.ExecuteNonQuery();
    }

    private static void CreateTableJet(string tableName, OleDbParameter[] columns, OleDbConnection connection)
    {
        var cmdText = "create table " + tableName + "(";
        var i = 0;
        var count = columns.Length;

        foreach (var column in columns)
        {
            string sqlType;

            switch (column.OleDbType)
            {
                case OleDbType.Char:
                    sqlType = "char(" + column.Size + ")";
                    break;

                case OleDbType.DBDate:
                    sqlType = "datetime";
                    break;

                case OleDbType.DBTimeStamp:
                    //sqlType = "timestamp";
                    sqlType = "datetime";
                    break;

                case OleDbType.UnsignedTinyInt:
                    sqlType = "tinyint";
                    break;

                case OleDbType.Integer:
                    sqlType = "int";
                    break;

                case OleDbType.Numeric:
                    //sqlType = "numeric(" + column.Precision + "," + column.Scale + ")";
                    sqlType = "numeric";
                    break;

                case OleDbType.Decimal:
                    //sqlType = "decimal(" + column.Precision + "," + column.Scale + ")";
                    sqlType = "numeric";
                    break;

                case OleDbType.VarChar:
                    sqlType = "varchar(" + column.Size + ")";
                    break;

                default:
                    sqlType = "varchar(255)";
                    break;
            }

            cmdText += "[" + column.SourceColumn + "] " + sqlType;

            if (column.IsNullable)
                cmdText += " NULL";
            else
                cmdText += " NOT NULL";

            if (i < count - 1)
                cmdText += ",";

            cmdText += Environment.NewLine;

            i++;
        }

        cmdText += ")";

        var command = connection.CreateCommand();
        command.CommandText = cmdText;
        command.ExecuteNonQuery();
    }

    private static void CreateTable(string tableName, OleDbParameter[] columns, OleDbConnection connection)
    {
        switch (connection.Provider)
        {
            case "SQLOLEDB.1":
                CreateTableSql(tableName, columns, connection);
                break;

            case "Microsoft.Jet.OLEDB.4.0":
                CreateTableJet(tableName, columns, connection);
                break;
        }
    }

    public static void CopyTable(DataTable sourceTable, OleDbConnection connection)
    {
        var adapter = new OleDbDataAdapter("select * from " + sourceTable.TableName + " where 0=1", connection);
        var destTable = new DataTable();
        adapter.Fill(destTable);

        foreach (DataRow sourceRow in sourceTable.Rows)
        {
            destTable.Rows.Add(sourceRow.ItemArray);
        }

        var builder = new OleDbCommandBuilder(adapter);
        adapter.Update(destTable);
    }

    public static void CopyTable(
        object adodbRecordset,
        string tableName,
        OleDbConnection connection)
    {
        var rs = (Recordset)adodbRecordset;
        var sourceTable = Convert(rs, out var columns);
        sourceTable.TableName = tableName;
        DropTable(tableName, connection);
        CreateTable(tableName, columns, connection);
        CopyTable(sourceTable, connection);
    }
}