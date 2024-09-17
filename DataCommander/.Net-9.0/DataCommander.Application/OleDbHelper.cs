using System;
using System.Data;
using System.Data.OleDb;
using ADODB;

namespace DataCommander.Application;

public static class OleDbHelper
{
    public static DataTable Convert(object adodbRecordset)
    {
        OleDbDataAdapter adapter = new OleDbDataAdapter();
        DataTable dataTable = new DataTable();
        adapter.Fill(dataTable, adodbRecordset);
        return dataTable;
    }

    [CLSCompliant(false)]
    public static DataTable Convert(_Recordset recordset, out OleDbParameter[] columns)
    {
        OleDbDataAdapter adapter = new OleDbDataAdapter();
        DataTable dataTable = new DataTable();
        adapter.Fill(dataTable, recordset);
        columns = new OleDbParameter[recordset.Fields.Count];
        int index = 0;

        foreach (Field field in recordset.Fields)
        {
            OleDbParameter param = new OleDbParameter
            {
                SourceColumn = field.Name,
                OleDbType = (OleDbType)field.Type
            };

            int size = field.DefinedSize;
            byte precision = field.Precision;

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
        OleDbCommand command = connection.CreateCommand();
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
        string cmdText = "create table " + tableName + "(";
        int i = 0;
        int count = columns.Length;

        foreach (OleDbParameter column in columns)
        {
            string sqlType = column.OleDbType switch
            {
                OleDbType.Char => "char(" + column.Size + ")",
                OleDbType.DBDate => "datetime",
                OleDbType.DBTimeStamp => "datetime",//sqlType = "timestamp";
                OleDbType.UnsignedTinyInt => "tinyint",
                OleDbType.Integer => "int",
                OleDbType.Numeric => "numeric(" + column.Precision + "," + column.Scale + ")",
                OleDbType.Decimal => "decimal(" + column.Precision + "," + column.Scale + ")",
                OleDbType.VarChar => "varchar(" + column.Size + ")",
                _ => "varchar(255)",
            };
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

        OleDbCommand command = connection.CreateCommand();
        command.CommandText = cmdText;
        command.ExecuteNonQuery();
    }

    private static void CreateTableJet(string tableName, OleDbParameter[] columns, OleDbConnection connection)
    {
        string cmdText = "create table " + tableName + "(";
        int i = 0;
        int count = columns.Length;

        foreach (OleDbParameter column in columns)
        {
            string sqlType = column.OleDbType switch
            {
                OleDbType.Char => "char(" + column.Size + ")",
                OleDbType.DBDate => "datetime",
                OleDbType.DBTimeStamp => "datetime",//sqlType = "timestamp";
                OleDbType.UnsignedTinyInt => "tinyint",
                OleDbType.Integer => "int",
                OleDbType.Numeric => "numeric",
                OleDbType.Decimal => "numeric",
                OleDbType.VarChar => "varchar(" + column.Size + ")",
                _ => "varchar(255)",
            };
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

        OleDbCommand command = connection.CreateCommand();
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
        OleDbDataAdapter adapter = new OleDbDataAdapter("select * from " + sourceTable.TableName + " where 0=1", connection);
        DataTable destTable = new DataTable();
        adapter.Fill(destTable);

        foreach (DataRow sourceRow in sourceTable.Rows)
        {
            destTable.Rows.Add(sourceRow.ItemArray);
        }

        OleDbCommandBuilder builder = new OleDbCommandBuilder(adapter);
        adapter.Update(destTable);
    }

    public static void CopyTable(
        object adodbRecordset,
        string tableName,
        OleDbConnection connection)
    {
        Recordset rs = (Recordset)adodbRecordset;
        DataTable sourceTable = Convert(rs, out OleDbParameter[]? columns);
        sourceTable.TableName = tableName;
        DropTable(tableName, connection);
        CreateTable(tableName, columns, connection);
        CopyTable(sourceTable, connection);
    }
}