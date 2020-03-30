﻿using System;
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
using DataCommander.Providers.Connection;
using DataCommander.Providers.Query;
using DataCommander.Providers2;
using DataCommander.Providers2.Connection;
using DataCommander.Providers2.FieldNamespace;
using Foundation.Collections.ReadOnly;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Diagnostics;
using Foundation.Linq;
using Foundation.Windows.Forms;
using OfficeOpenXml;

namespace DataCommander.Providers.ResultWriter
{
    internal class DataTableEditor : UserControl
    {
        #region Private Fields

        private readonly DbCommandBuilder _commandBuilder;
        private DoubleBufferedDataGridView _dataGrid;
        private string _tableName;
        private GetTableSchemaResult _tableSchema;
        private DataTable _dataTable;
        private ToolStripStatusLabel _statusBarPanel;
        private int _columnIndex;
        private string _columnName;
        private object _cellValue;
        private StringBuilder _statementStringBuilder;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly Container _components = new Container();

        #endregion

        #region Constructors

        public DataTableEditor(DbCommandBuilder commandBuilder, ColorTheme colorTheme)
        {
            _commandBuilder = commandBuilder;

            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // TODO: Add any initialization after the InitForm call
            GarbageMonitor.Default.Add("DataTableEditor", this);

            if (colorTheme != null)
                ColorThemeApplier.Apply(_dataGrid, colorTheme);
        }

        #endregion

        #region Properties

        public DataGridView DataGrid => _dataGrid;

        public DataTable DataTable
        {
            get => _dataTable;

            set
            {
                _dataTable = value;

                if (_dataTable != null)
                {
                    if (!_dataGrid.ReadOnly)
                    {
                        _dataTable.RowDeleting += dataTable_RowDeleting;
                        _dataTable.RowChanging += dataTable_RowChanging;
                        _dataGrid.DataError += dataGrid_DataError;
                    }

                    var ts = new DataGridTableStyle();
                    ts.MappingName = _dataTable.TableName;
                    // TODO
                    // dataGrid.TableStyles.Add(ts);                    

                    var graphics = CreateGraphics();
                    var font = _dataGrid.Font;

                    foreach (DataColumn dataColumn in _dataTable.Columns)
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

                            foreach (DataRow dataRow in _dataTable.Rows)
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
                        _dataGrid.Columns.Add(textBoxColumn);
                    }

                    _dataGrid.DataSource = value;
                    _dataGrid.Visible = false;
                    _dataGrid.Width = 2000;
                    _dataGrid.Visible = true;
                }
                else
                {
                    _dataGrid.DataSource = null;
                    _dataGrid.Rows.Clear();
                    _dataGrid.Columns.Clear();
                    _dataGrid.Dispose();
                }
            }
        }

        public bool ReadOnly
        {
            get => _dataGrid.ReadOnly;

            set
            {
                _dataGrid.ReadOnly = value;
                _dataGrid.AllowUserToAddRows = !value;
                _dataGrid.AllowUserToDeleteRows = !value;
            }
        }

        public string TableName
        {
            set => _tableName = value;
        }

        public GetTableSchemaResult TableSchema
        {
            set
            {
                _tableSchema = value;

                if (_tableSchema != null)
                {
                    var uniqueIndexColumns = UniqueIndexColumns.ToArray();
                    string message;

                    if (uniqueIndexColumns.Length > 0)
                        message = "The table has a primary key/unique index. Columns: " +
                                  uniqueIndexColumns.Select(r => r.ColumnName).Aggregate((n1, n2) => $"{n1},{n2}");
                    else
                        message = "WARNING: The table has no primary key/unique index.";

                    var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                    queryForm.AddInfoMessage(InfoMessageFactory.Create(InfoMessageSeverity.Information, null, message));
                }
            }
        }

        public ToolStripStatusLabel StatusBarPanel
        {
            set => _statusBarPanel = value;
        }

        private IEnumerable<Column> UniqueIndexColumns =>
            _tableSchema.UniqueIndexColumns.Select(i =>
            {
                var column = _tableSchema.Columns.First(j => j.ColumnId == i.ColumnId);
                return column;
            });

        private object CurrentCellValue
        {
            get
            {
                var cell = _dataGrid.CurrentCell;
                var rowNumber = cell.RowIndex;
                var columnNumber = cell.ColumnIndex;
                var dataRow = _dataTable.DefaultView[rowNumber].Row;
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
                if (_components != null)
                    _components.Dispose();

                if (_dataGrid != null)
                {
                    _dataGrid.Dispose();
                    _dataGrid = null;
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
            this._dataGrid = new DoubleBufferedDataGridView();
            GarbageMonitor.Default.Add("dataGrid", "DoubleBufferedDataGridView", 0, this._dataGrid);
            this._dataGrid.PublicDoubleBuffered = true;
            this._dataGrid.AllowUserToOrderColumns = true;
            ((System.ComponentModel.ISupportInitialize) (this._dataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this._dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._dataGrid.Location = new System.Drawing.Point(0, 0);
            this._dataGrid.Name = "_dataGrid";
            dataGridViewCellStyle1.NullValue = "(null)";
            this._dataGrid.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this._dataGrid.Size = new System.Drawing.Size(424, 208);
            this._dataGrid.TabIndex = 0;
            this._dataGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGrid_MouseDown);
            // 
            // DataTableViewer
            // 
            this.Controls.Add(this._dataGrid);
            this.Name = "DataTableViewer";
            this.Size = new System.Drawing.Size(424, 208);
            ((System.ComponentModel.ISupportInitialize) (this._dataGrid)).EndInit();
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
                valueString = "null";
            else
            {
                var type = (Type) column.ExtendedProperties[0];

                if (type == null)
                    type = column.DataType;

                var typeCode = Type.GetTypeCode(type);

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        var boolValue = (bool) Convert.ChangeType(value, typeof(bool));
                        valueString = boolValue ? "1" : "0";
                        break;

                    case TypeCode.DateTime:
                        valueString = (string) value;
                        var succeeded = DateTimeField.TryParse(valueString, out var dateTime);

                        if (succeeded)
                            valueString = dateTime.ToSqlConstant();

                        break;

                    case TypeCode.String:
                        var stringValue = (string) value;
                        valueString = stringValue.ToNullableNVarChar();
                        break;

                    default:
                        if (type == typeof(Guid))
                        {
                            var guid = (Guid) value;
                            valueString = guid.ToString().ToNullableVarChar();
                        }
                        else
                            valueString = value.ToString();

                        break;
                }
            }

            return valueString;
        }

        private string GetWhere(DataRow row)
        {
            var columns = _tableSchema.Columns;
            var stringBuilder = new StringBuilder();
            var first = true;
            var uniqueIndexColumns = UniqueIndexColumns.ToReadOnlyList();
            if (uniqueIndexColumns.Count == 0)
                uniqueIndexColumns = columns.ToReadOnlyList();

            foreach (var uniqueIndexColumn in uniqueIndexColumns)
            {
                var columnId = uniqueIndexColumn.ColumnId;
                var columnName = uniqueIndexColumn.ColumnName;

                if (first)
                {
                    first = false;
                    stringBuilder.Append(" where ");
                }
                else
                    stringBuilder.Append(" and ");

                var contains = row.Table.Columns.Contains(columnName);
                if (contains)
                {
                    var dataColumn = _dataTable.Columns[columnName];
                    var value = row[dataColumn, DataRowVersion.Current];
                    var valueString = ToString(dataColumn, value);
                    var operatorString = value == DBNull.Value ? "is" : "=";
                    var quotedColumnName = _commandBuilder.QuoteIdentifier(columnName);
                    stringBuilder.AppendFormat("{0} {1} {2}", quotedColumnName, operatorString, valueString);
                }
                else
                    stringBuilder.AppendFormat("/* the column {0} is part of the primary key but it is missing from the query. */", columnName);
            }

            return stringBuilder.ToString();
        }

        private void HandleDataRowAddAction(DataRow dataRow)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            var valid = true;

            foreach (var column in _tableSchema.Columns)
            {
                var columnName = column.ColumnName;
                var hasDefault = column.DefaultObjectId != 0;
                var isNullable = column.IsNullable == true;
                var isComputed = column.IsComputed;
                var value = dataRow[columnName];

                if (value == DBNull.Value && !isNullable && !hasDefault && !isComputed)
                {
                    valid = false;
                    stringBuilder.AppendFormat("Column '{0}' is not nullable, has not default, is not computed.\r\n", columnName);
                }
            }

            if (valid)
            {
                var table = dataRow.Table;
                stringBuilder = new StringBuilder();
                stringBuilder.AppendFormat("\r\ninsert into {0}(", _tableName);
                var first = true;
                foreach (var column in _tableSchema.Columns)
                {
                    if (!column.IsComputed)
                    {
                        if (first)
                            first = false;
                        else
                            stringBuilder.Append(',');

                        stringBuilder.Append(column.ColumnName);
                    }
                }

                stringBuilder.Append(") values(");
                first = true;

                var sequence = new Sequence();
                foreach (var column in _tableSchema.Columns)
                {
                    var columnIndex = sequence.Next();
                    if (!column.IsComputed)
                    {
                        var hasDeafult = column.DefaultObjectId != 0;

                        if (first)
                            first = false;
                        else
                            stringBuilder.Append(',');

                        var value = dataRow[columnIndex];
                        string valueString;

                        if (value == DBNull.Value && hasDeafult)
                            valueString = "default";
                        else
                        {
                            var dataColumn = _dataTable.Columns[columnIndex];
                            valueString = ToString(dataColumn, value);
                        }

                        stringBuilder.Append(valueString);
                    }
                }

                stringBuilder.Append(')');
            }

            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.AppendQueryText(stringBuilder.ToString());
        }

        private void HandleDataRowChangeAction(DataRow dataRow)
        {
            //var builder = this.provi
            //IProvider provider;
            //provider.DbProviderFactory.CreateCommandBuilder().QuoteIdentifier(

            var sb = new StringBuilder();
            sb.AppendFormat("\r\nupdate {0}", _tableName);
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
                        @equals = currentValue == DBNull.Value;
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
                    @equals = Equals(currentValue, proposedValue);

                if (!@equals)
                {
                    changed = true;

                    if (first)
                    {
                        first = false;
                        sb.Append(" set ");
                    }
                    else
                        sb.Append(", ");

                    var valueString = ToString(column, proposedValue);
                    var quotedColumnName = _commandBuilder.QuoteIdentifier(column.ColumnName);
                    sb.AppendFormat("{0} = {1}", quotedColumnName, valueString);
                }
            }

            if (changed)
            {
                var where = GetWhere(dataRow);
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
                    HandleDataRowAddAction(e.Row);
                    break;

                case DataRowAction.Change:
                    HandleDataRowChangeAction(e.Row);
                    break;

                default:
                    break;
            }
        }

        private void dataTable_RowDeleting(object sender, DataRowChangeEventArgs e)
        {
            if (_statementStringBuilder == null)
                _statementStringBuilder = new StringBuilder();

            _statementStringBuilder.AppendFormat("\r\ndelete from {0}", _tableName);
            var where = GetWhere(e.Row);
            _statementStringBuilder.Append(@where);

            if (_dataGrid.SelectedRows.Count == 1)
            {
                var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                var text = _statementStringBuilder.ToString();
                _statementStringBuilder = null;
                queryForm.AppendQueryText(text);
            }
        }

        private void CopyColumnNames_Click(object sender, EventArgs e)
        {
            var columnNames =
                (from c in _dataGrid.Columns.Cast<DataGridViewColumn>()
                    where c.Visible
                    orderby c.DisplayIndex
                    select c.DataPropertyName);
            var s = string.Join(",", columnNames);
            Clipboard.SetDataObject(s, true, 5, 200);
        }

        private void CopyColumnName_Click(object sender, EventArgs e) => Clipboard.SetDataObject(_columnName, true, 5, 200);

        private int[] GetColumnIndexes()
        {
            return
                (from c in _dataGrid.Columns.Cast<DataGridViewColumn>()
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
                Cursor = Cursors.WaitCursor;

                var path = saveFileDialog.FileName;

                _statusBarPanel.Text = "Saving table...";

                Task.Factory.StartNew(() =>
                {
                    var stopwatch = Stopwatch.StartNew();

                    try
                    {
                        switch (saveFileDialog.FilterIndex)
                        {
                            case 1:
                                var columnIndexes = GetColumnIndexes();
                                using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
                                    HtmlFormatter.Write(_dataTable.DefaultView, columnIndexes, streamWriter);
                                break;

                            case 2:
                                using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
                                    streamWriter.Write(_dataTable.DefaultView.ToStringTableString());
                                break;

                            case 3:
                                using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
                                    Writer.Write(_dataTable.DefaultView, '\t', "\r\n", streamWriter);
                                break;

                            case 4: // XML Spreadsheet 2007(*.xlsx)
                                var fileInfo = new FileInfo(path);
                                using (var excelPackage = new ExcelPackage(fileInfo))
                                {
                                    var worksheet = excelPackage.Workbook.Worksheets.Add($"Worksheet {LocalTime.Default.Now:yyyy-MM-dd HHmmss}");
                                    worksheet.View.FreezePanes(2, 1);

                                    var dataView = _dataTable.DefaultView;
                                    var rowCount = dataView.Count;
                                    var columnCount = _dataTable.Columns.Count;

                                    for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
                                    {
                                        var cell = worksheet.Cells[1, columnIndex + 1];
                                        cell.Value = _dataTable.Columns[columnIndex].ColumnName;
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
                        _statusBarPanel.Text = $"Table saved successfully in {StopwatchTimeSpan.ToString(stopwatch.ElapsedTicks, 3)} seconds.";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                });
            }
        }

        private void CopyTable_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                var dataObject = new MyDataObject(_dataTable.DefaultView, GetColumnIndexes());
                Clipboard.SetDataObject(dataObject);
                if (_statusBarPanel != null)
                {
                    _statusBarPanel.Text =
                        string.Format("Data copied to clipboard. Data is available in 3 formats: HTML, TAB separated text, FIXED width text.");
                }
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void EditDataViewProperties_Click(object sender, EventArgs e)
        {
            var properties = new DataViewProperties();
            var dataView = _dataTable.DefaultView;
            properties.RowFilter = dataView.RowFilter;
            properties.Sort = dataView.Sort;
            var form = new DataViewPropertiesForm(properties);
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                dataView.RowFilter = properties.RowFilter;
                dataView.Sort = properties.Sort;
                _statusBarPanel.Text = $"RowFilter = \"{properties.RowFilter}\" applied. dataView.Count: {dataView.Count}";
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
            saveFileDialog.FileName = _columnName;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var binaryField = (BinaryField) _cellValue;
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
            var binaryField = (BinaryField) _cellValue;
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
                var streamField = (StreamField) _cellValue;
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
            var s = _cellValue as string;

            if (s == null)
            {
                var stringField = (StringField) _cellValue;
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

            if (_cellValue.IfAsNotNull(delegate(StringField stringField)
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
            _dataTable.DefaultView.RowFilter = null;
        }

        private void ApplyRowFilter(string rowFilter)
        {
            try
            {
                var dataView = _dataTable.DefaultView;
                dataView.RowFilter = rowFilter;

                if (_statusBarPanel != null)
                {
                    _statusBarPanel.Text =
                        $"RowFilter ({rowFilter}) applied. {dataView.Count} row(s) found from {_dataTable.Rows.Count} row(s).";
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
            ApplyRowFilter(rowFilter);
        }

        private void Find_Click(object sender, EventArgs e)
        {
            var form = new FindTextForm();
            form.Text = "dataView.RowFilter = ...";
            form.FindText = _columnName;

            if (form.ShowDialog() == DialogResult.OK)
            {
                ApplyRowFilter(form.FindText);
            }
        }

        private void CopyArrayField_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            var array = (Array) _cellValue;

            for (var i = 0; i < array.Length; i++)
            {
                var obj = array.GetValue(i);
                sb.AppendLine(obj.ToString());
            }

            Clipboard.SetText(sb.ToString());
        }

        private void HideColumn_Click(object sender, EventArgs e)
        {
            var column = _dataGrid.Columns[_columnIndex];
            column.Visible = false;
        }

        private void UnhideAllColumns_Click(object sender, EventArgs e)
        {
            foreach (var column in _dataGrid.Columns.Cast<DataGridViewColumn>().Where(c => !c.Visible))
            {
                column.Visible = true;
            }
        }

        private void CopyTableAsXml_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _statusBarPanel.Text = "Copying table to clipboard as XML...";
                var textWriter = new StringWriter();
                var xmlWriter = new XmlTextWriter(textWriter);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.Indentation = 2;
                xmlWriter.IndentChar = ' ';
                xmlWriter.WriteStartElement("table");

                var columns = _dataTable.Columns;
                var columnCount = columns.Count;

                foreach (var row in _dataGrid.Rows.Cast<DataGridViewRow>().Where(r => r.Visible))
                {
                    xmlWriter.WriteStartElement("row");
                    for (var columnIndex = 0; columnIndex < columnCount; columnIndex++)
                    {
                        if (_dataGrid.Columns[_columnIndex].Visible)
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
                _statusBarPanel.Text = "Table succesfully copied to clipboard as XML.";
            }
        }

        private void HideRows_Click(object sender, EventArgs e)
        {
            DataGridViewRow currentRow = null;

            foreach (DataGridViewRow row in _dataGrid.SelectedRows)
            {
                if (row == _dataGrid.CurrentRow)
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
                _dataGrid.CurrentCell = null;
                currentRow.Visible = false;
            }
        }

        private void UnhideRows_Click(object sender, EventArgs e)
        {
            foreach (var row in _dataGrid.Rows.Cast<DataGridViewRow>().Where(r => !r.Visible))
            {
                row.Visible = true;
            }
        }

        private void dataGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var menu = new ContextMenuStrip(_components);
                var rowFilter = _dataTable.DefaultView.RowFilter;
                ToolStripMenuItem menuItem;
                if (!string.IsNullOrEmpty(rowFilter))
                {
                    menuItem = new ToolStripMenuItem($"Remove rowFilter: {rowFilter}", null, RemoveRowFilter_Click);
                    menu.Items.Add(menuItem);
                }

                var hitTestInfo = _dataGrid.HitTest(e.X, e.Y);
                switch (hitTestInfo.Type)
                {
                    case DataGridViewHitTestType.TopLeftHeader:
                        menuItem = new ToolStripMenuItem("Copy column names", null, CopyColumnNames_Click);
                        menu.Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem("&Save table as", null, SaveTableAs_Click);
                        menu.Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem("&Copy table", null, CopyTable_Click);
                        menu.Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem("Copy table as XML", null, CopyTableAsXml_Click);
                        menu.Items.Add(menuItem);

                        menuItem = new ToolStripMenuItem("Edit dataview properties", null, EditDataViewProperties_Click);
                        menu.Items.Add(menuItem);

                        var any = _dataGrid.Columns.Cast<DataGridViewColumn>().Any(c => !c.Visible);
                        if (any)
                        {
                            menuItem = new ToolStripMenuItem("Unhide all columns", null, UnhideAllColumns_Click);
                            menu.Items.Add(menuItem);
                        }

                        any = _dataGrid.Rows.Cast<DataGridViewRow>().Any(r => !r.Visible);
                        if (any)
                        {
                            menuItem = new ToolStripMenuItem("Unhide all rows", null, UnhideRows_Click);
                            menu.Items.Add(menuItem);
                        }

                        break;

                    case DataGridViewHitTestType.ColumnHeader:
                    {
                        _columnIndex = hitTestInfo.ColumnIndex;
                        _columnName = _dataTable.Columns[_columnIndex].ColumnName;
                        menuItem = new ToolStripMenuItem($"Copy column name '{_columnName}'", null, CopyColumnName_Click);
                        menu.Items.Add(menuItem);
                        menuItem = new ToolStripMenuItem("Hide column", null, HideColumn_Click);
                        menu.Items.Add(menuItem);
                    }

                        break;

                    case DataGridViewHitTestType.Cell:
                    {
                        rowFilter = null;
                        var rowNumber = hitTestInfo.RowIndex;
                        var columnNumber = hitTestInfo.ColumnIndex;

                        var dataRow = _dataTable.DefaultView[rowNumber].Row;
                        _columnName = _dataTable.Columns[columnNumber].ColumnName;

                        if (_columnName.IndexOf('!') >= 0)
                        {
                            _columnName = $"[{_columnName}]";
                        }

                        menuItem = new ToolStripMenuItem("&Find", null, Find_Click);
                        menu.Items.Add(menuItem);

                        _cellValue = dataRow[columnNumber];
                        var type = _cellValue.GetType();
                        var fieldType = FieldTypeDictionary.Instance.GetValueOrDefault(type);

                        switch (fieldType)
                        {
                            case FieldType.StringField:
                                var value = ((StringField) _cellValue).Value;
                                if (value != null && value.Length < 256)
                                    rowFilter = $"[{_columnName}] = '{value}'";
                                break;

                            case FieldType.DateTimeField:
                                rowFilter = null;
                                break;

                            default:
                                if (_cellValue == DBNull.Value)
                                    rowFilter = $"[{_columnName}] is null";
                                else
                                {
                                    var typeCode = Type.GetTypeCode(type);
                                    string valueStr;

                                    switch (typeCode)
                                    {
                                        case TypeCode.String:
                                            valueStr = (string) _cellValue;

                                            if (valueStr.Length < 256)
                                            {
                                                valueStr = $"'{_cellValue}'";
                                                rowFilter = $"[{_columnName}] = {valueStr}";
                                            }

                                            break;

                                        case TypeCode.Object:
                                            if (type == typeof(Guid))
                                            {
                                                valueStr = $"'{_cellValue}'";
                                            }
                                            else
                                            {
                                                valueStr = _cellValue.ToString();
                                            }

                                            rowFilter = $"[{_columnName}] = {valueStr}";
                                            break;

                                        default:
                                            valueStr = _cellValue.ToString();
                                            rowFilter = $"[{_columnName}] = {valueStr}";
                                            break;
                                    }
                                }

                                break;
                        }

                        if (rowFilter != null)
                        {
                            menuItem = new ToolStripMenuItem(rowFilter, null, RowFilter_Click);
                            menu.Items.Add(menuItem);
                        }

                        if (_cellValue != DBNull.Value)
                        {
                            switch (fieldType)
                            {
                                case FieldType.BinaryField:
                                    menuItem = new ToolStripMenuItem("Save binary field as", null, SaveBinaryField_Click);
                                    menu.Items.Add(menuItem);
                                    menuItem = new ToolStripMenuItem("Open as Excel file", null, OpenAsExcelFile_Click);
                                    menu.Items.Add(menuItem);
                                    break;

                                case FieldType.StreamField:
                                    menuItem = new ToolStripMenuItem("Save stream field as", null, SaveStreamField_Click);
                                    menu.Items.Add(menuItem);
                                    break;

                                case FieldType.StringField:
                                {
                                    var stringField = (StringField) _cellValue;
                                    var value = stringField.Value;
                                    var length = value != null ? value.Length : 0;
                                    menuItem = new ToolStripMenuItem("Copy string field", null, CopyStringField_Click);
                                    menu.Items.Add(menuItem);

                                    menuItem = new ToolStripMenuItem(
                                        $"Save string field (length: {length}) as",
                                        null,
                                        SaveStringField_Click);

                                    menu.Items.Add(menuItem);
                                }
                                    break;

                                case FieldType.String:
                                {
                                    var value = (string) _cellValue;
                                    var length = value.Length;

                                    menuItem = new ToolStripMenuItem("Copy string field", null, CopyStringField_Click);
                                    menu.Items.Add(menuItem);

                                    menuItem = new ToolStripMenuItem(
                                        $"Save string field (length: {length}) as",
                                        null,
                                        SaveStringField_Click);

                                    menu.Items.Add(menuItem);
                                }
                                    break;

                                case FieldType.StringArray:
                                    menuItem = new ToolStripMenuItem("Copy string[] field", null, CopyArrayField_Click);
                                    menu.Items.Add(menuItem);
                                    break;
                            }
                        }
                    }
                        break;

                    case DataGridViewHitTestType.RowHeader:
                        menuItem = new ToolStripMenuItem("Hide rows", null, HideRows_Click);
                        menu.Items.Add(menuItem);
                        break;

                    default:
                        break;
                }

                var pos = new Point(e.X, e.Y);
                menu.Show(_dataGrid, pos);
            }
        }

        #endregion
    }
}