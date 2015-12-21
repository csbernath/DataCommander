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
        private readonly List<DoubleBufferedDataGridView> dataGridViews = new List<DoubleBufferedDataGridView>();

        public DataGridViewResultWriter()
        {
        }

        public List<DoubleBufferedDataGridView> DataGridViews
        {
            get
            {
                return this.dataGridViews;
            }
        }

        #region IResultWriter Members

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
            DataColumnSchema schema = new DataColumnSchema(schemaDataRow);
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
            this.dataGridViews.Add(dataGridView);
        }

        void IResultWriter.FirstRowReadBegin()
        {
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            var dataGridView = this.dataGridViews[this.dataGridViews.Count - 1];
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