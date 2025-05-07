using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Application.ResultWriter;

internal sealed class DataGridViewResultWriter : IResultWriter
{
    public DataGridViewResultWriter()
    {
    }

    public List<DoubleBufferedDataGridView> DataGridViews { get; } = [];

    void IResultWriter.Begin(IProvider provider)
    {
    }

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
    {
    }

    void IResultWriter.AfterExecuteReader()
    {
    }

    void IResultWriter.AfterCloseReader(int affectedRows)
    {
    }

    private static DataGridViewColumn ToDataGridViewColumn(DataRow schemaDataRow)
    {
        var schema = FoundationDbColumnFactory.Create(schemaDataRow);
        var column = new DataGridViewTextBoxColumn
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Name = schema.ColumnName
        };
        return column;
    }

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        var dataGridView = new DoubleBufferedDataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSize = false
        };

        var columns =
            (from schemaRow in schemaTable.AsEnumerable()
                select ToDataGridViewColumn(schemaRow)).ToArray();
        dataGridView.Columns.AddRange(columns);
        DataGridViews.Add(dataGridView);
    }

    void IResultWriter.FirstRowReadBegin()
    {
    }

    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
    {
    }

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        var dataGridView = DataGridViews[^1];
        var targetRows = dataGridView.Rows;
        for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            var sourceRow = rows[rowIndex];
            var targetRow = new DataGridViewRow();
            var cells = targetRow.Cells;
            foreach (var sourceValue in sourceRow)
            {
                var cell = new DataGridViewTextBoxCell
                {
                    Value = sourceValue
                };
                cells.Add(cell);
            }

            targetRows.Add(targetRow);
        }
    }

    void IResultWriter.WriteTableEnd()
    {
    }

    void IResultWriter.WriteParameters(IDataParameterCollection parameters)
    {
    }

    void IResultWriter.End()
    {
    }
}