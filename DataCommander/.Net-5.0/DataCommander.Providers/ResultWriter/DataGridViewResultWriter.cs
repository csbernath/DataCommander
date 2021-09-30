using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class DataGridViewResultWriter : IResultWriter
    {
        public DataGridViewResultWriter()
        {
        }

        public List<DoubleBufferedDataGridView> DataGridViews { get; } = new();

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider)
        {
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
        {
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
        }

        private static DataGridViewColumn ToDataGridViewColumn(DataRow schemaDataRow)
        {
            var schema = FoundationDbColumnFactory.Create(schemaDataRow);
            var column = new DataGridViewTextBoxColumn()
            {
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                Name = schema.ColumnName
            };
            return column;
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            var dataGridView = new DoubleBufferedDataGridView();
            dataGridView.AllowUserToAddRows = false;
            dataGridView.AllowUserToDeleteRows = false;
            dataGridView.AutoSize = false;

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
            var dataGridView = DataGridViews[DataGridViews.Count - 1];
            var targetRows = dataGridView.Rows;
            for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var sourceRow = rows[rowIndex];
                var targetRow = new DataGridViewRow();
                var cells = targetRow.Cells;
                for (var columnIndex = 0; columnIndex < sourceRow.Length; columnIndex++)
                {
                    var sourceValue = sourceRow[columnIndex];
                    var cell = new DataGridViewTextBoxCell();
                    cell.Value = sourceValue;
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

        #endregion
    }
}