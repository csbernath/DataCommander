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
        FoundationDbColumn schema = FoundationDbColumnFactory.Create(schemaDataRow);
        DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn()
        {
            AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            Name = schema.ColumnName
        };
        return column;
    }

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        DoubleBufferedDataGridView dataGridView = new DoubleBufferedDataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoSize = false
        };

        DataGridViewColumn[] columns =
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
        DoubleBufferedDataGridView dataGridView = DataGridViews[^1];
        DataGridViewRowCollection targetRows = dataGridView.Rows;
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            object[] sourceRow = rows[rowIndex];
            DataGridViewRow targetRow = new DataGridViewRow();
            DataGridViewCellCollection cells = targetRow.Cells;
            foreach (object sourceValue in sourceRow)
            {
                DataGridViewTextBoxCell cell = new DataGridViewTextBoxCell
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