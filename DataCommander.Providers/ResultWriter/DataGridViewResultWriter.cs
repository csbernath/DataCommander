namespace DataCommander.Providers
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal sealed class DataGridViewResultWriter : IResultWriter
    {
        public DataGridViewResultWriter()
        {
        }

        public List<DoubleBufferedDataGridView> DataGridViews { get; } = new List<DoubleBufferedDataGridView>();

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
            DbColumn schema = new DbColumn(schemaDataRow);
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
            this.DataGridViews.Add(dataGridView);
        }

        void IResultWriter.FirstRowReadBegin()
        {
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            var dataGridView = this.DataGridViews[this.DataGridViews.Count - 1];
            var targetRows = dataGridView.Rows;
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                var sourceRow = rows[rowIndex];
                var targetRow = new DataGridViewRow();
                var cells = targetRow.Cells;
                for (int columnIndex = 0; columnIndex < sourceRow.Length; columnIndex++)
                {
                    object sourceValue = sourceRow[columnIndex];
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