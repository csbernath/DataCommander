using System;
using System.Data;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Data;
using Foundation.Diagnostics;

namespace DataCommander.Application.ResultWriter;

internal sealed class DataSetResultWriter(Action<InfoMessage> addInfoMessage, bool showShemaTable) : IResultWriter
{
    private readonly IResultWriter _logResultWriter = new LogResultWriter(addInfoMessage);
    private IProvider _provider;
    private DataTable _dataTable;
    private int _tableIndex = -1;

    public DataSet DataSet { get; private set; }

    void IResultWriter.Begin(IProvider provider)
    {
        _logResultWriter.Begin(provider);
        _provider = provider;
    }

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command) => _logResultWriter.BeforeExecuteReader(command);

    void IResultWriter.AfterExecuteReader()
    {
        _logResultWriter.AfterExecuteReader();
        DataSet = new DataSet();
    }

    void IResultWriter.AfterCloseReader(int affectedRows) => _logResultWriter.AfterCloseReader(affectedRows);

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        ++_tableIndex;
        _logResultWriter.WriteTableBegin(schemaTable);
        CreateTable(schemaTable);
    }

    void IResultWriter.FirstRowReadBegin() => _logResultWriter.FirstRowReadBegin();

    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames) => _logResultWriter.FirstRowReadEnd(dataTypeNames);

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        _logResultWriter.WriteRows(rows, rowCount);
        DataRowCollection targetRows = _dataTable.Rows;
        for (int i = 0; i < rowCount; i++)
            targetRows.Add(rows[i]);
    }

    void IResultWriter.WriteTableEnd()
    {
        _logResultWriter.WriteTableEnd();
        GarbageMonitor.Default.Add("DataSetResultWriter", "System.Data.DataTable", _dataTable.Rows.Count, _dataTable);
    }

    void IResultWriter.WriteParameters(IDataParameterCollection parameters)
    {
        // TODO TextResultWriter.WriteParameters(parameters, textWriter, queryForm);
    }

    void IResultWriter.End() => _logResultWriter.End();

    private void CreateTable(DataTable schemaTable)
    {
        string tableName = schemaTable.TableName;
        if (tableName == "SchemaTable")
            tableName = $"Table {_tableIndex}";
        if (showShemaTable)
        {
            schemaTable.TableName = $"Schema {_tableIndex}";
            DataSet.Tables.Add(schemaTable);
        }

        _dataTable = DataSet.Tables.Add();
        if (!string.IsNullOrEmpty(tableName))
            _dataTable.TableName = tableName;

        foreach (DataRow schemaRow in schemaTable.Rows)
        {
            FoundationDbColumn dataColumnSchema = FoundationDbColumnFactory.Create(schemaRow);
            string columnName = dataColumnSchema.ColumnName;
            int columnSize = dataColumnSchema.ColumnSize;
            Type dataType = _provider.GetColumnType(dataColumnSchema);

            DataColumn dataColumn;
            int n = 2;
            string columnName2 = columnName;

            while (true)
            {
                if (_dataTable.Columns.Contains(columnName2))
                {
                    columnName2 = columnName + n;
                    n++;
                }
                else
                {
                    columnName = columnName2;

                    if (dataType != null)
                        dataColumn = _dataTable.Columns.Add(columnName, dataType);
                    else
                        dataColumn = _dataTable.Columns.Add(columnName);

                    dataColumn.ExtendedProperties.Add("ColumnName", dataColumnSchema.ColumnName);

                    //dataColumn.AllowDBNull = sr.AllowDBNull == true;                                
                    //dataColumn.Unique = sr.IsUnique == true; // TFS provider does not support this column
                    dataColumn.ExtendedProperties.Add(0, schemaRow["DataType"]);
                    break;
                }
            }
        }
    }
}