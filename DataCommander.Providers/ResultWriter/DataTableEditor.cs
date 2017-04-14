namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Xml;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Data.SqlClient;
    using DataCommander.Foundation.Diagnostics;
    using DataCommander.Foundation.Linq;
    using DataCommander.Foundation.Windows.Forms;
    using OfficeOpenXml;

    /// <summary>
    /// Summary description for DataTableViewer.
    /// </summary>
    internal class DataTableEditor : UserControl
    {
        #region Private Fields

        private readonly DbCommandBuilder commandBuilder;
        private DoubleBufferedDataGridView dataGrid;
        private string tableName;
        private DataSet tableSchema;
        private DataTable dataTable;
        private ToolStripStatusLabel statusBarPanel;
        private int columnIndex;
        private string columnName;
        private object cellValue;
        private StringBuilder statementStringBuilder;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container components = new Container();

        #endregion

        #region Constructors

        public DataTableEditor(DbCommandBuilder commandBuilder)
        {
            this.commandBuilder = commandBuilder;

            // This call is required by the Windows.Forms Form Designer.
            this.InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            GarbageMonitor.Add("DataTableEditor", this);
        }

        #endregion

        #region Properties

        public DataGridView DataGrid => this.dataGrid;

        public DataTable DataTable
        {
            get => this.dataTable;

            set
            {
                this.dataTable = value;

                if (this.dataTable != null)
                {
                    if (!this.dataGrid.ReadOnly)
                    {
                        this.dataTable.RowDeleting += this.dataTable_RowDeleting;
                        this.dataTable.RowChanging += this.dataTable_RowChanging;
                        this.dataGrid.DataError += this.dataGrid_DataError;
                    }

                    var ts = new DataGridTableStyle();
                    ts.MappingName = this.dataTable.TableName;
                    // TODO
                    // dataGrid.TableStyles.Add(ts);                    

                    var graphics = this.CreateGraphics();
                    var font = this.dataGrid.Font;

                    foreach (DataColumn dataColumn in this.dataTable.Columns)
                    {
                        var textBoxColumn = new DataGridViewTextBoxColumn();
                        textBoxColumn.DataPropertyName = dataColumn.ColumnName;

                        string columnName;
                        if (dataColumn.ExtendedProperties.ContainsKey("ColumnName"))
                        {
                            columnName = (string) dataColumn.ExtendedProperties["ColumnName"];
                            if (string.IsNullOrEmpty(columnName))
                            {
                                columnName = "(no column name)";
                            }
                        }
                        else
                        {
                            columnName = dataColumn.ColumnName;
                        }

                        textBoxColumn.HeaderText = columnName;
                        var maxWidth = graphics.MeasureString(columnName, font).Width;
                        var type = (Type) dataColumn.ExtendedProperties[0];

                        if (type == null)
                        {
                            type = (Type) dataColumn.DataType;
                        }

                        var typeCode = Type.GetTypeCode(type);

                        if (typeCode == TypeCode.Byte ||
                            typeCode == TypeCode.SByte ||
                            typeCode == TypeCode.Int16 ||
                            typeCode == TypeCode.Int32 ||
                            typeCode == TypeCode.Int64 ||
                            typeCode == TypeCode.UInt16 ||
                            typeCode == TypeCode.UInt32 ||
                            typeCode == TypeCode.UInt64 ||
                            typeCode == TypeCode.Decimal ||
                            typeCode == TypeCode.Single ||
                            typeCode == TypeCode.Double)
                        {
                            textBoxColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        else if (typeCode == TypeCode.DateTime)
                        {
                            textBoxColumn.DefaultCellStyle.Format = "yyyy.MM.dd HH:mm:ss.fff";
                        }

                        if (true)
                        {
                            var rowIndex = 0;

                            foreach (DataRow dataRow in this.dataTable.Rows)
                            {
                                var s = dataRow[dataColumn].ToString();
                                var length = s.Length;

                                if (length <= 256)
                                {
                                    var width = graphics.MeasureString(s, font).Width;


                                    if (width > maxWidth)
                                    {
                                        maxWidth = width;
                                    }
                                }

                                if (rowIndex == 999)
                                {
                                    break;
                                }

                                rowIndex++;
                            }

                            if (maxWidth > 250)
                            {
                                maxWidth = 250;
                            }
                        }

                        textBoxColumn.Width = (int) Math.Ceiling(maxWidth) + 5;
                        this.dataGrid.Columns.Add(textBoxColumn);
                    }

                    this.dataGrid.DataSource = value;
                    this.dataGrid.Visible = false;
                    this.dataGrid.Width = 2000;
                    this.dataGrid.Visible = true;
                }
                else
                {
                    this.dataGrid.DataSource = null;
                    this.dataGrid.Rows.Clear();
                    this.dataGrid.Columns.Clear();
                    this.dataGrid.Dispose();
                }
            }
        }

        public bool ReadOnly
        {
            get => this.dataGrid.ReadOnly;

            set
            {
                this.dataGrid.ReadOnly = value;
                this.dataGrid.AllowUserToAddRows = !value;
                this.dataGrid.AllowUserToDeleteRows = !value;
            }
        }

        public string TableName
        {
            set => this.tableName = value;
        }

        public DataSet TableSchema
        {
            set
            {
                this.tableSchema = value;

                if (this.tableSchema != null)
                {
                    var uniqueIndexColumns = this.UniqueIndexColumns.ToArray();
                    string message;

                    if (uniqueIndexColumns.Length > 0)
                    {
                        message = "The table has a primary key/unique index. Columns: " +
                                  uniqueIndexColumns.Select(r => (string) r["ColumnName"]).Aggregate((n1, n2) => $"{n1},{n2}");
                    }
                    else
                    {
                        message = "WARNING: The table has no primary key/unique index.";
                    }

                    var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                    queryForm.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, message));
                }
            }
        }

        public ToolStripStatusLabel StatusBarPanel
        {
            set => this.statusBarPanel = value;
        }

        private IEnumerable<DataRow> UniqueIndexColumns
        {
            get
            {
                foreach (DataRow dataRow in this.tableSchema.Tables[1].Rows)
                {
                    var columnOrdinal = dataRow.Field<int>(0);
                    var column = this.tableSchema.Tables[0].Rows[columnOrdinal - 1];
                    yield return column;
                }
            }
        }

        private object CurrentCellValue
        {
            get
            {
                var cell = this.dataGrid.CurrentCell;
                var rowNumber = cell.RowIndex;
                var columnNumber = cell.ColumnIndex;
                var dataRow = this.dataTable.DefaultView[rowNumber].Row;
                var value = dataRow[columnNumber];
                return value;
            }
        }

        #endregion

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    this.components.Dispose();
                }

                if (this.dataGrid != null)
                {
                    this.dataGrid.Dispose();
                    this.dataGrid = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            var dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGrid = new DoubleBufferedDataGridView();
            GarbageMonitor.Add("dataGrid", "DoubleBufferedDataGridView", 0, this.dataGrid);
            this.dataGrid.PublicDoubleBuffered = true;
            this.dataGrid.AllowUserToOrderColumns = true;
            ((System.ComponentModel.ISupportInitialize) (this.dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.Name = "dataGrid";
            dataGridViewCellStyle1.NullValue = "(null)";
            this.dataGrid.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGrid.Size = new System.Drawing.Size(424, 208);
            this.dataGrid.TabIndex = 0;
            this.dataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGrid_MouseDown);
            // 
            // DataTableViewer
            // 
            this.Controls.Add(this.dataGrid);
            this.Name = "DataTableViewer";
            this.Size = new System.Drawing.Size(424, 208);
            ((System.ComponentModel.ISupportInitialize) (this.dataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        #region Private Methods

        private void dataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString());
            e.ThrowException = false;
            e.Cancel = true;
        }

        private string ToString(DataColumn column, object value)
        {
            string valueString;

            if (value == DBNull.Value)
            {
                valueString = "null";
            }
            else
            {
                var type = (Type) column.ExtendedProperties[0];

                if (type == null)
                {
                    type = column.DataType;
                }

                var typeCode = Type.GetTypeCode(type);

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        var boolValue = (bool) Convert.ChangeType(value, typeof(bool));
                        valueString = boolValue ? "1" : "0";
                        break;

                    case TypeCode.DateTime:
                        valueString = (string) value;
                        DateTime dateTime;
                        var succeeded = DateTimeField.TryParse(valueString, out dateTime);

                        if (succeeded)
                        {
                            valueString = dateTime.ToTSqlDateTime();
                        }

                        break;

                    case TypeCode.String:
                        var stringValue = (string) value;
                        valueString = stringValue.ToTSqlNVarChar();
                        break;

                    default:
                        if (type == typeof(Guid))
                        {
                            var guid = (Guid) value;
                            valueString = guid.ToString().ToTSqlVarChar();
                        }
                        else
                        {
                            valueString = value.ToString();
                        }

                        break;
                }
            }

            return valueString;
        }

        private string GetWhere(DataRow row)
        {
            var columns = this.tableSchema.Tables[0];
            var sb = new StringBuilder();
            var first = true;
            var uniqueIndexColumns = this.UniqueIndexColumns.ToArray();

            var schema = uniqueIndexColumns.Length > 0
                ? uniqueIndexColumns
                : this.tableSchema.Tables[0].Rows.Cast<DataRow>().ToArray();

            foreach (var uniqueIndexColumn in schema)
            {
                var columnOrdinal = (short) uniqueIndexColumn["Columnordinal"];
                var column = columns.Rows[columnOrdinal - 1];
                var columnName = (string) column[SchemaTableColumn.ColumnName];

                if (first)
                {
                    first = false;
                    sb.Append(" where ");
                }
                else
                {
                    sb.Append(" and ");
                }

                var contains = row.Table.Columns.Contains(columnName);

                if (contains)
                {
                    var dataColumn = this.dataTable.Columns[columnName];
                    var value = row[dataColumn, DataRowVersion.Current];
                    var valueString = this.ToString(dataColumn, value);
                    var operatorString = value == DBNull.Value ? "is" : "=";
                    var quotedColumnName = this.commandBuilder.QuoteIdentifier(columnName);
                    sb.AppendFormat("{0} {1} {2}", quotedColumnName, operatorString, valueString);
                }
                else
                {
                    sb.AppendFormat("/* the column {0} is part of the primary key but it is missing from the query. */", columnName);
                }
            }

            return sb.ToString();
        }

        private void HandleDataRowAddAction(DataRow dataRow)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            var valid = true;

            foreach (DataRow schemaRow in this.tableSchema.Tables[0].Rows)
            {
                var columnName = (string) schemaRow["ColumnName"];
                var hasDefault = schemaRow.Field<bool>("HasDefault");
                var isNullable = schemaRow.Field<bool>("IsNullable");
                var hasAutomaticValue = schemaRow.Field<bool>("HasAutomaticValue");
                var value = dataRow[columnName];

                if (value == DBNull.Value && !isNullable && !hasDefault && !hasAutomaticValue)
                {
                    valid = false;
                    sb.AppendFormat("Column '{0}' is not nullable, has not default, has not automatic value.\r\n", columnName);
                }
            }

            if (valid)
            {
                var table = dataRow.Table;
                sb = new StringBuilder();
                sb.AppendFormat("\r\ninsert into {0}(", this.tableName);
                var first = true;
                var schemaRows = this.tableSchema.Tables[0].Rows;

                foreach (DataColumn column in table.Columns)
                {
                    var schemaRow = schemaRows[column.Ordinal];
                    var hasAutomaticValue = schemaRow.Field<bool>("HasAutomaticValue");

                    if (!hasAutomaticValue)
                    {
                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(',');
                        }

                        sb.Append(column.ColumnName);
                    }
                }

                sb.Append(") values(");
                first = true;

                foreach (DataColumn column in table.Columns)
                {
                    var schemaRow = schemaRows[column.Ordinal];
                    var hasAutomaticValue = schemaRow.Field<bool>("HasAutomaticValue");

                    if (!hasAutomaticValue)
                    {
                        var hasDeafult = schemaRow.Field<bool>("HasDefault");

                        if (first)
                        {
                            first = false;
                        }
                        else
                        {
                            sb.Append(',');
                        }

                        var value = dataRow[column];
                        string valueString;

                        if (value == DBNull.Value && hasDeafult)
                        {
                            valueString = "default";
                        }
                        else
                        {
                            valueString = this.ToString(column, value);
                        }

                        sb.Append(valueString);
                    }
                }

                sb.Append(')');
            }

            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.AppendQueryText(sb.ToString());
        }

        private void HandleDataRowChangeAction(DataRow dataRow)
        {
            //var builder = this.provi
            //IProvider provider;
            //provider.DbProviderFactory.CreateCommandBuilder().QuoteIdentifier(

            var sb = new StringBuilder();
            sb.AppendFormat("\r\nupdate {0}", this.tableName);
            var first = true;
            var changed = false;

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                var currentValue = dataRow[column, DataRowVersion.Current];
                var proposedValue = dataRow[column, DataRowVersion.Proposed];
                var comparable = currentValue as IComparable;
                bool equals;

                if (comparable != null)
                {
                    if (proposedValue == DBNull.Value)
                    {
                        @equals = currentValue == DBNull.Value;
                    }
                    else
                    {
                        var currentType = currentValue.GetType();
                        var proposedType = proposedValue.GetType();
                        int c;

                        try
                        {
                            c = comparable.CompareTo(proposedValue);
                        }
                        catch
                        {
                            var convertedValue = Convert.ChangeType(proposedValue, currentType);
                            c = comparable.CompareTo(convertedValue);
                        }

                        @equals = c == 0;
                    }
                }
                else
                {
                    @equals = Equals(currentValue, proposedValue);
                }

                if (!@equals)
                {
                    changed = true;

                    if (first)
                    {
                        first = false;
                        sb.Append(" set ");
                    }
                    else
                    {
                        sb.Append(", ");
                    }

                    var valueString = this.ToString(column, proposedValue);
                    var quotedColumnName = this.commandBuilder.QuoteIdentifier(column.ColumnName);
                    sb.AppendFormat("{0} = {1}", quotedColumnName, valueString);
                }
            }

            if (changed)
            {
                var where = this.GetWhere(dataRow);
                sb.Append(@where);
                var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                var text = sb.ToString();
                queryForm.AppendQueryText(text);
            }
        }

        private void dataTable_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            switch (e.Action)
            {
                case DataRowAction.Add:
                    this.HandleDataRowAddAction(e.Row);
                    break;

                case DataRowAction.Change:
                    this.HandleDataRowChangeAction(e.Row);
                    break;

                default:
                    break;
            }
        }

        private void dataTable_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (this.statementStringBuilder == null)
            {
                this.statementStringBuilder = new StringBuilder();
            }

            this.statementStringBuilder.AppendFormat("\r\ndelete from {0}", this.tableName);
            var where = this.GetWhere(e.Row);
            this.statementStringBuilder.Append(@where);

            if (this.dataGrid.SelectedRows.Count == 1)
            {
                var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                var text = this.statementStringBuilder.ToString();
                this.statementStringBuilder = null;
                queryForm.AppendQueryText(text);
            }
        }

        private void CopyColumnNames_Click(object sender, EventArgs e)
        {
            var columnNames =
            (from c in this.dataGrid.Columns.Cast<DataGridViewColumn>()
                where c.Visible
                orderby c.DisplayIndex
                select c.DataPropertyName);
            var s = string.Join(",", columnNames);
            Clipboard.SetDataObject(s, true, 5, 200);
        }

        private void CopyColumnName_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(this.columnName, true, 5, 200);
        }

        private int[] GetColumnIndexes()
        {
            return
            (from c in this.dataGrid.Columns.Cast<DataGridViewColumn>()
                where c.Visible
                orderby c.DisplayIndex
                select c.Index).ToArray();
        }

        private void SaveTableAs_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save table";
            saveFileDialog.Filter =
                "HTML (*.htm)|*.htm|Fixed Width Columns (*.txt)|*.txt|Tab Separated Values (*.tsv)|*.tsv|XML Spreadsheet 2007(*.xlsx)|*.xlsx";
            saveFileDialog.FilterIndex = 5;
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.Cursor = Cursors.WaitCursor;

                var path = saveFileDialog.FileName;

                this.statusBarPanel.Text = "Saving table...";

                Task.Factory.StartNew(() =>
                {
                    var stopwatch = Stopwatch.StartNew();

                    try
                    {
                        switch (saveFileDialog.FilterIndex)
                        {
                            case 1:
                                var columnIndexes = this.GetColumnIndexes();
                                using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
                                {
                                    HtmlFormatter.Write(this.dataTable.DefaultView, columnIndexes, streamWriter);
                                }
                                break;

                            case 2:
                                using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
                                {
                                    streamWriter.Write(this.dataTable.DefaultView.ToStringTableString());
                                }
                                break;

                            case 3:
                                using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
                                {
                                    Database.Write(this.dataTable.DefaultView, '\t', "\r\n", streamWriter);
                                }
                                break;

                            case 4: // XML Spreadsheet 2007(*.xlsx)
                                var fileInfo = new FileInfo(path);
                                using (var excelPackage = new ExcelPackage(fileInfo))
                                {
                                    var worksheet = excelPackage.Workbook.Worksheets.Add($"Worksheet {LocalTime.Default.Now:yyyy-MM-dd HHmmss}");
                                    worksheet.View.FreezePanes(2, 1);

                                    var dataView = this.dataTable.DefaultView;
                                    var rowCount = dataView.Count;
                                    var columnCount = this.dataTable.Columns.Count;

                                    for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
                                    {
                                        var cell = worksheet.Cells[1, columnIndex + 1];
                                        cell.Value = this.dataTable.Columns[columnIndex].ColumnName;
                                        cell.Style.Font.Bold = true;
                                        // cell.Style.Border.Bottom.Style = ExcelBorderStyle.Medium;
                                    }

                                    for (var rowIndex = 0; rowIndex < rowCount; rowIndex++)
                                    {
                                        var dataRow = dataView[rowIndex];
                                        for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
                                        {
                                            var value = dataRow[columnIndex];
                                            var cell = worksheet.Cells[rowIndex + 2, columnIndex + 1];
                                            cell.Value = value;

                                            var type = value.GetType();
                                            var typeCode = Type.GetTypeCode(type);
                                            switch (typeCode)
                                            {
                                                case TypeCode.DateTime:
                                                    cell.Style.Numberformat.Format = "mm-dd-yy";
                                                    break;

                                                default:
                                                    break;
                                            }
                                        }
                                    }

                                    excelPackage.Save();
                                }
                                break;

                            default:
                                throw new ArgumentException();
                        }

                        stopwatch.Stop();
                        this.statusBarPanel.Text = $"Table saved successfully in {StopwatchTimeSpan.ToString(stopwatch.ElapsedTicks, 3)} seconds.";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                });
            }
        }

        private void CopyTable_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                var dataObject = new MyDataObject(this.dataTable.DefaultView, this.GetColumnIndexes());
                Clipboard.SetDataObject(dataObject);
                if (this.statusBarPanel != null)
                {
                    this.statusBarPanel.Text =
                        string.Format("Data copied to clipboard. Data is available in 3 formats: HTML, TAB separated text, FIXED width text.");
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void EditDataViewProperties_Click(object sender, EventArgs e)
        {
            var properties = new DataViewProperties();
            var dataView = this.dataTable.DefaultView;
            properties.RowFilter = dataView.RowFilter;
            properties.Sort = dataView.Sort;
            var form = new DataViewPropertiesForm(properties);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                dataView.RowFilter = properties.RowFilter;
                dataView.Sort = properties.Sort;
                this.statusBarPanel.Text = $"RowFilter = \"{properties.RowFilter}\" applied. dataView.Count: {dataView.Count}";
            }
        }

        private void SaveBinaryField_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Binary Field";
            saveFileDialog.Filter = "Binary Files (*.bin)|*.bin";
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.DefaultExt = "bin";
            saveFileDialog.FileName = this.columnName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var binaryField = (BinaryField) this.cellValue;
                var path = saveFileDialog.FileName;

                using (var fileStream = File.Create(path))
                {
                    var bytes = binaryField.Value;
                    fileStream.Write(bytes, 0, bytes.Length);
                }
            }
        }

        private void OpenAsExcelFile_Click(object sender, EventArgs e)
        {
            var binaryField = (BinaryField) this.cellValue;
            var path = Path.Combine(Path.GetTempPath(), Path.GetTempFileName() + ".zip");
            File.WriteAllBytes(path, binaryField.Value);
            Process.Start(path);
        }

        private void SaveStreamField_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save Binary Field";
            saveFileDialog.Filter = "Binary Files (*.bin)|*.bin";
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true;
            saveFileDialog.DefaultExt = "bin";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var streamField = (StreamField) this.cellValue;
                var path = saveFileDialog.FileName;
                var source = streamField.Stream;

                using (var target = File.Create(path))
                {
                    var buffer = new byte[4096];

                    while (true)
                    {
                        var readCount = source.Read(buffer, 0, buffer.Length);

                        if (readCount == 0)
                        {
                            break;
                        }

                        target.Write(buffer, 0, readCount);
                    }
                }
            }
        }

        private void CopyStringField_Click(object sender, EventArgs e)
        {
            var s = this.cellValue as string;

            if (s == null)
            {
                var stringField = (StringField) this.cellValue;
                s = stringField.Value;
            }

            Clipboard.SetText(s);
        }

        private void SaveStringField_Click(object sender, EventArgs e)
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save String Field";
            saveFileDialog.AddExtension = true;
            saveFileDialog.OverwritePrompt = true;

            string value = null;
            Encoding encoding = null;

            if (this.cellValue.IfAsNotNull(delegate(StringField stringField)
            {
                var stringReader = new StringReader(stringField.Value);
                var xmlTextReader = new XmlTextReader(stringReader);
                bool isXml;

                try
                {
                    var read = xmlTextReader.Read();
                    isXml = true;
                }
                catch
                {
                    isXml = false;
                }

                if (isXml)
                {
                    saveFileDialog.Filter = "XML files (*.xml)|*.xml";
                    saveFileDialog.DefaultExt = "xml";
                    encoding = xmlTextReader.Encoding;
                }
                else
                {
                    saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
                    saveFileDialog.DefaultExt = "txt";
                    encoding = Encoding.UTF8;
                }

                value = stringField.Value;
            }))
            {
            }

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var path = saveFileDialog.FileName;
                using (var streamWriter = new StreamWriter(path, false, encoding))
                {
                    streamWriter.Write(value);
                }
            }
        }

        private void RemoveRowFilter_Click(object sender, EventArgs e)
        {
            this.dataTable.DefaultView.RowFilter = null;
        }

        private void ApplyRowFilter(string rowFilter)
        {
            try
            {
                var dataView = this.dataTable.DefaultView;
                dataView.RowFilter = rowFilter;

                if (this.statusBarPanel != null)
                {
                    this.statusBarPanel.Text =
                        $"RowFilter ({rowFilter}) applied. {dataView.Count} row(s) found from {this.dataTable.Rows.Count} row(s).";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void RowFilter_Click(object sender, EventArgs e)
        {
            var menuItem = (ToolStripMenuItem) sender;
            var rowFilter = menuItem.Text;
            this.ApplyRowFilter(rowFilter);
        }

        private void Find_Click(object sender, EventArgs e)
        {
            var form = new FindTextForm();
            form.Text = "dataView.RowFilter = ...";
            form.FindText = this.columnName;

            if (form.ShowDialog() == DialogResult.OK)
            {
                this.ApplyRowFilter(form.FindText);
            }
        }

        private void CopyArrayField_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            var array = (Array) this.cellValue;

            for (var i = 0; i < array.Length; i++)
            {
                var obj = array.GetValue(i);
                sb.AppendLine(obj.ToString());
            }

            Clipboard.SetText(sb.ToString());
        }

        private void HideColumn_Click(object sender, EventArgs e)
        {
            var column = this.dataGrid.Columns[this.columnIndex];
            column.Visible = false;
        }

        private void UnhideAllColumns_Click(object sender, EventArgs e)
        {
            foreach (var column in this.dataGrid.Columns.Cast<DataGridViewColumn>().Where(c => !c.Visible))
            {
                column.Visible = true;
            }
        }

        private void CopyTableAsXml_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                this.statusBarPanel.Text = "Copying table to clipboard as XML...";
                var textWriter = new StringWriter();
                var xmlWriter = new XmlTextWriter(textWriter);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 2;
                xmlWriter.IndentChar = ' ';
                xmlWriter.WriteStartElement("table");

                var columns = this.dataTable.Columns;
                var columnCount = columns.Count;

                foreach (var row in this.dataGrid.Rows.Cast<DataGridViewRow>().Where(r => r.Visible))
                {
                    xmlWriter.WriteStartElement("row");
                    for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
                    {
                        if (this.dataGrid.Columns[this.columnIndex].Visible)
                        {
                            var column = columns[columnIndex];
                            var dataRowView = (DataRowView) row.DataBoundItem;
                            var value = dataRowView[columnIndex];
                            if (value != DBNull.Value)
                            {
                                xmlWriter.WriteStartElement(column.ColumnName);
                                string valueString;
                                var convertible = value as IConvertible;
                                if (convertible != null)
                                {
                                    valueString = convertible.ToString(null);
                                }
                                else
                                {
                                    valueString = value.ToString();
                                }

                                //TypeCode typeCode = Type.GetTypeCode(column.DataType);
                                //switch (typeCode)
                                //{
                                //    case TypeCode.Object:
                                //        if (value is Guid)
                                //        {
                                //            Guid guid = (Guid)value;
                                //            value = guid.ToString();
                                //        }
                                //        else
                                //        {
                                //            DateTimeField dateTimeField = value as DateTimeField;
                                //            if (dateTimeField != null)
                                //            {
                                //                value = dateTimeField.Value;
                                //            }
                                //            else
                                //            {
                                //                StringField stringField = value as StringField;
                                //                if (stringField != null)
                                //                {
                                //                    value = stringField.Value;
                                //                }
                                //                else
                                //                {
                                //                    BinaryField binaryField o
                                //                    throw new NotImplementedException();
                                //                }
                                //            }
                                //        }
                                //        break;
                                //    default:
                                //        break;
                                //}
                                xmlWriter.WriteValue(valueString);
                                xmlWriter.WriteEndElement();
                            }
                        }
                    }
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.Close();
                var xml = textWriter.ToString();
                Clipboard.SetDataObject(xml, true, 5, 200);
                this.statusBarPanel.Text = "Table succesfully copied to clipboard as XML.";
            }
        }

        private void HideRows_Click(object sender, EventArgs e)
        {
            DataGridViewRow currentRow = null;

            foreach (DataGridViewRow row in this.dataGrid.SelectedRows)
            {
                if (row == this.dataGrid.CurrentRow)
                {
                    currentRow = row;
                }
                else
                {
                    row.Visible = false;
                }
            }

            if (currentRow != null)
            {
                this.dataGrid.CurrentCell = null;
                currentRow.Visible = false;
            }
        }

        private void UnhideRows_Click(object sender, EventArgs e)
        {
            foreach (var row in this.dataGrid.Rows.Cast<DataGridViewRow>().Where(r => !r.Visible))
            {
                row.Visible = true;
            }
        }

        private void dataGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = new ContextMenuStrip(this.components);
                var rowFilter = this.dataTable.DefaultView.RowFilter;
                ToolStripMenuItem menuItem;
                if (!string.IsNullOrEmpty(rowFilter))
                {
                    menuItem = new ToolStripMenuItem($"Remove rowFilter: {rowFilter}", null, this.RemoveRowFilter_Click);
                    menu.Items.Add(menuItem);
                }

                var hitTestInfo = this.dataGrid.HitTest(e.X, e.Y);
                switch (hitTestInfo.Type)
                {
                    case DataGridViewHitTestType.TopLeftHeader:
                        menuItem = new ToolStripMenuItem("Copy column names", null, this.CopyColumnNames_Click);
                        menu.Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem("&Save table as", null, this.SaveTableAs_Click);
                        menu.Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem("&Copy table", null, this.CopyTable_Click);
                        menu.Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem("Copy table as XML", null, this.CopyTableAsXml_Click);
                        menu.Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem("Edit dataview properties", null, this.EditDataViewProperties_Click);
                        menu.Items.Add(menuItem);

                        var any = this.dataGrid.Columns.Cast<DataGridViewColumn>().Any(c => !c.Visible);
                        if (any)
                        {
                            menuItem = new ToolStripMenuItem("Unhide all columns", null, this.UnhideAllColumns_Click);
                            menu.Items.Add(menuItem);
                        }
                        any = this.dataGrid.Rows.Cast<DataGridViewRow>().Any(r => !r.Visible);
                        if (any)
                        {
                            menuItem = new ToolStripMenuItem("Unhide all rows", null, this.UnhideRows_Click);
                            menu.Items.Add(menuItem);
                        }
                        break;

                    case DataGridViewHitTestType.ColumnHeader:
                    {
                        this.columnIndex = hitTestInfo.ColumnIndex;
                        this.columnName = this.dataTable.Columns[this.columnIndex].ColumnName;
                        menuItem = new ToolStripMenuItem($"Copy column name '{this.columnName}'", null, this.CopyColumnName_Click);
                        menu.Items.Add(menuItem);
                        menuItem = new ToolStripMenuItem("Hide column", null, this.HideColumn_Click);
                        menu.Items.Add(menuItem);
                    }

                        break;

                    case DataGridViewHitTestType.Cell:
                    {
                        rowFilter = null;
                        var rowNumber = hitTestInfo.RowIndex;
                        var columnNumber = hitTestInfo.ColumnIndex;

                        var dataRow = this.dataTable.DefaultView[rowNumber].Row;
                        this.columnName = this.dataTable.Columns[columnNumber].ColumnName;

                        if (this.columnName.IndexOf('!') >= 0)
                        {
                            this.columnName = $"[{this.columnName}]";
                        }

                        menuItem = new ToolStripMenuItem("&Find", null, this.Find_Click);
                        menu.Items.Add(menuItem);

                        this.cellValue = dataRow[columnNumber];
                        var type = this.cellValue.GetType();
                        var fieldType = FieldTypeDictionary.Instance.GetValueOrDefault(type);

                        switch (fieldType)
                        {
                            case FieldType.StringField:
                                var value = ((StringField) this.cellValue).Value;

                                if (value != null && value.Length < 256)
                                {
                                    rowFilter = $"[{this.columnName}] = '{value}'";
                                }
                                break;

                            case FieldType.DateTimeField:
                                rowFilter = null;
                                break;

                            default:
                                if (this.cellValue == DBNull.Value)
                                {
                                    rowFilter = $"[{this.columnName}] is null";
                                }
                                else
                                {
                                    var typeCode = Type.GetTypeCode(type);
                                    string valueStr;

                                    switch (typeCode)
                                    {
                                        case TypeCode.String:
                                            valueStr = (string) this.cellValue;

                                            if (valueStr.Length < 256)
                                            {
                                                valueStr = $"'{this.cellValue}'";
                                                rowFilter = $"[{this.columnName}] = {valueStr}";
                                            }

                                            break;

                                        case TypeCode.Object:
                                            if (type == typeof(Guid))
                                            {
                                                valueStr = $"'{this.cellValue}'";
                                            }
                                            else
                                            {
                                                valueStr = this.cellValue.ToString();
                                            }

                                            rowFilter = $"[{this.columnName}] = {valueStr}";
                                            break;

                                        default:
                                            valueStr = this.cellValue.ToString();
                                            rowFilter = $"[{this.columnName}] = {valueStr}";
                                            break;
                                    }
                                }
                                break;
                        }

                        if (rowFilter != null)
                        {
                            menuItem = new ToolStripMenuItem(rowFilter, null, this.RowFilter_Click);
                            menu.Items.Add(menuItem);
                        }

                        if (this.cellValue != DBNull.Value)
                        {
                            switch (fieldType)
                            {
                                case FieldType.BinaryField:
                                    menuItem = new ToolStripMenuItem("Save binary field as", null, this.SaveBinaryField_Click);
                                    menu.Items.Add(menuItem);
                                    menuItem = new ToolStripMenuItem("Open as Excel file", null, this.OpenAsExcelFile_Click);
                                    menu.Items.Add(menuItem);
                                    break;

                                case FieldType.StreamField:
                                    menuItem = new ToolStripMenuItem("Save stream field as", null, this.SaveStreamField_Click);
                                    menu.Items.Add(menuItem);
                                    break;

                                case FieldType.StringField:
                                {
                                    var stringField = (StringField) this.cellValue;
                                    var value = stringField.Value;
                                    var length = value != null ? value.Length : 0;
                                    menuItem = new ToolStripMenuItem("Copy string field", null, new EventHandler(this.CopyStringField_Click));
                                    menu.Items.Add(menuItem);

                                    menuItem = new ToolStripMenuItem(
                                        $"Save string field (length: {length}) as",
                                        null,
                                        new EventHandler(this.SaveStringField_Click));

                                    menu.Items.Add(menuItem);
                                }
                                    break;

                                case FieldType.String:
                                {
                                    var value = (string) this.cellValue;
                                    var length = value.Length;

                                    menuItem = new ToolStripMenuItem("Copy string field", null, new EventHandler(this.CopyStringField_Click));
                                    menu.Items.Add(menuItem);

                                    menuItem = new ToolStripMenuItem(
                                        $"Save string field (length: {length}) as",
                                        null,
                                        new EventHandler(this.SaveStringField_Click));

                                    menu.Items.Add(menuItem);
                                }
                                    break;

                                case FieldType.StringArray:
                                    menuItem = new ToolStripMenuItem("Copy string[] field", null, this.CopyArrayField_Click);
                                    menu.Items.Add(menuItem);
                                    break;
                            }
                        }
                    }
                        break;

                    case DataGridViewHitTestType.RowHeader:
                        menuItem = new ToolStripMenuItem("Hide rows", null, this.HideRows_Click);
                        menu.Items.Add(menuItem);
                        break;

                    default:
                        break;
                }

                var pos = new Point(e.X, e.Y);
                menu.Show(this.dataGrid, pos);
            }
        }

        #endregion
    }
}