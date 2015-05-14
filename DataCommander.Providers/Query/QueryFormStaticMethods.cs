namespace DataCommander.Providers.Query
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;

    internal static class QueryFormStaticMethods
    {
        private static HtmlTextBox CreateHtmlTextBoxFromDataTable(DataTable dataTable)
        {
            string fileName = Path.GetTempFileName();
            var fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
            using (StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
            {
                int[] columnIndexes = new int[dataTable.Columns.Count];
                for (int i = 0; i < columnIndexes.Length; i++)
                {
                    columnIndexes[i] = i;
                }
                HtmlFormatter.Write(dataTable.DefaultView, columnIndexes, streamWriter);
            }
            HtmlTextBox htmlTextBox = new HtmlTextBox();
            htmlTextBox.Navigate(fileName);

            //while (webBrowser.ReadyState != SHDocVw.tagREADYSTATE.READYSTATE_COMPLETE)
            //{
            //    Application.DoEvents();
            //}

            //File.Delete(fileName);
            return htmlTextBox;
        }

        public static Control CreateControlFromDataTable(
            DbCommandBuilder commandBuilder,
            DataTable dataTable,
            DataSet schemaTable,
            ResultWriterType tableStyle,
            bool readOnly,
            ToolStripStatusLabel toolStripStatusLabel)
        {
            Control control;

            switch (tableStyle)
            {
                case ResultWriterType.DataGrid:
                    control = CreateDataTableEditorFromDataTable(commandBuilder, dataTable, schemaTable, readOnly, toolStripStatusLabel);
                    break;

                case ResultWriterType.Html:
                    control = CreateHtmlTextBoxFromDataTable(dataTable);
                    break;

                case ResultWriterType.ListView:
                    control = CreateListViewFromDataTable(dataTable);
                    break;

                default:
                    throw new NotImplementedException();
            }
            return control;
        }

        private static DataTableEditor CreateDataTableEditorFromDataTable(
            DbCommandBuilder commandBuilder,
            DataTable dataTable,
            DataSet tableSchema,
            bool readOnly,
            ToolStripStatusLabel toolStripStatusLabel)
        {
            var editor = new DataTableEditor(commandBuilder);
            editor.ReadOnly = readOnly;
            editor.DataTable = dataTable;
            editor.TableName = dataTable.TableName;
            editor.TableSchema = tableSchema;
            editor.StatusBarPanel = toolStripStatusLabel;
            return editor;
        }

        private static ListView CreateListViewFromDataTable(DataTable dataTable)
        {
            var listView = new ListView
            {
                View = View.Details,
                GridLines = true,
                AllowColumnReorder = true,
                Font = new Font("Tahoma", 8),
                FullRowSelect = true
            };

            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                var columnHeader = new ColumnHeader
                {
                    Text = dataColumn.ColumnName,
                    Width = -2
                };

                Type type = (Type)dataColumn.ExtendedProperties[0];

                if (type == null)
                {
                    type = dataColumn.DataType;
                }

                columnHeader.TextAlign = GetHorizontalAlignment(type);

                listView.Columns.Add(columnHeader);
            }

            int count = dataTable.Columns.Count;
            var items = new string[count];

            foreach (DataRow dataRow in dataTable.Rows)
            {
                for (int i = 0; i < count; i++)
                {
                    object value = dataRow[i];

                    if (value == DBNull.Value)
                    {
                        items[i] = "(null)";
                    }
                    else
                    {
                        items[i] = value.ToString();
                    }
                }

                var listViewItem = new ListViewItem(items);
                listView.Items.Add(listViewItem);
            }

            return listView;
        }

        public static HorizontalAlignment GetHorizontalAlignment(Type type)
        {
            HorizontalAlignment align;

            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                    align = HorizontalAlignment.Right;
                    break;

                default:
                    align = HorizontalAlignment.Left;
                    break;
            }

            return align;
        }

        public static bool FindText(
            DataView dataView,
            IStringMatcher matcher,
            ref int rowIndex,
            ref int columnIndex)
        {
            bool found = false;
            var dataTable = dataView.Table;
            var dataRows = dataTable.Rows;
            int rowCount = dataView.Count;
            int columnCount = dataTable.Columns.Count;
            object currentValueObject = dataTable.DefaultView[rowIndex][columnIndex];
            string currentValue;
            if (currentValueObject is StringField)
            {
                var stringField = currentValueObject as StringField;
                currentValue = stringField.Value;
            }
            else
            {
                currentValue = currentValueObject.ToString();
            }

            if (matcher.IsMatch(currentValue))
            {
                if (columnIndex < columnCount - 1)
                {
                    columnIndex++;
                }
                else if (rowIndex < rowCount - 1)
                {
                    rowIndex++;
                }
            }

            if (rowIndex == 0)
            {
                for (int i = columnIndex + 1; i < dataTable.Columns.Count; i++)
                {
                    DataColumn dataColumn = dataTable.Columns[i];
                    found = matcher.IsMatch(dataColumn.ColumnName);

                    if (found)
                    {
                        columnIndex = i;
                        break;
                    }
                }
            }

            if (!found)
            {
                DataRow dataRow = dataTable.DefaultView[rowIndex].Row;

                while (true)
                {
                    found = matcher.IsMatch(dataRow[columnIndex].ToString());

                    if (found)
                    {
                        //cell.DataGridView.Rows[row].Cells[col].Selected = true;
                        // TODO
                        // cell.RowIndex = row;
                        // cell.ColumnIndex = col;
                        break;
                    }

                    if ((columnIndex + 1)%columnCount != 0)
                    {
                        columnIndex++;
                    }
                    else
                    {
                        rowIndex++;
                        columnIndex = 0;

                        if (rowIndex < rowCount)
                        {
                            dataRow = dataTable.DefaultView[rowIndex].Row;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return found;
        }
    }
}