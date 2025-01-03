using System;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;
using DataCommander.Application.ResultWriter;
using DataCommander.Api;
using DataCommander.Api.FieldReaders;
using DataCommander.Api.Query;

namespace DataCommander.Application.Query;

internal static class QueryFormStaticMethods
{
    //private static HtmlTextBox CreateHtmlTextBoxFromDataTable(DataTable dataTable)
    //{
    //    var fileName = Path.GetTempFileName();
    //    var fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
    //    using (var streamWriter = new StreamWriter(fileStream, Encoding.UTF8))
    //    {
    //        var columnIndexes = new int[dataTable.Columns.Count];
    //        for (var i = 0; i < columnIndexes.Length; i++)
    //            columnIndexes[i] = i;
    //        HtmlFormatter.Write(dataTable.DefaultView, columnIndexes, streamWriter);
    //    }

    //    var htmlTextBox = new HtmlTextBox();
    //    htmlTextBox.Navigate(fileName);

    //    return htmlTextBox;
    //}

    public static Control CreateControlFromDataTable(IQueryForm queryForm, DbCommandBuilder commandBuilder, DataTable dataTable,
        GetTableSchemaResult getTableSchemaResult, ResultWriterType tableStyle, bool readOnly, ColorTheme colorTheme)
    {
        Control control;

        switch (tableStyle)
        {
            case ResultWriterType.DataGrid:
                control = CreateDataTableEditorFromDataTable(queryForm, commandBuilder, dataTable, getTableSchemaResult, readOnly, colorTheme);
                break;

            //case ResultWriterType.Html:
            //    control = CreateHtmlTextBoxFromDataTable(dataTable);
            //    break;

            case ResultWriterType.ListView:
                control = CreateListViewFromDataTable(dataTable);
                break;

            default:
                throw new NotImplementedException();
        }

        return control;
    }

    private static DataTableEditor CreateDataTableEditorFromDataTable(IQueryForm queryForm, DbCommandBuilder commandBuilder, DataTable dataTable,
        GetTableSchemaResult getTableSchemaResult, bool readOnly, ColorTheme colorTheme)
    {
        var editor = new DataTableEditor(queryForm, commandBuilder, colorTheme);
        editor.ReadOnly = readOnly;
        editor.DataTable = dataTable;
        editor.TableName = dataTable.TableName;
        editor.TableSchema = getTableSchemaResult;
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

            var type = (Type)dataColumn.ExtendedProperties[0];

            if (type == null)
            {
                type = dataColumn.DataType;
            }

            columnHeader.TextAlign = GetHorizontalAlignment(type);

            listView.Columns.Add(columnHeader);
        }

        var count = dataTable.Columns.Count;
        var items = new string[count];

        foreach (DataRow dataRow in dataTable.Rows)
        {
            for (var i = 0; i < count; i++)
            {
                var value = dataRow[i];

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

        var typeCode = Type.GetTypeCode(type);

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

    public static bool FindText(DataView dataView, IStringMatcher matcher, ref int rowIndex, ref int columnIndex)
    {
        var found = false;
        var dataTable = dataView.Table;
        var dataRows = dataTable.Rows;
        var rowCount = dataView.Count;
        var columnCount = dataTable.Columns.Count;
        var currentValueObject = dataTable.DefaultView[rowIndex][columnIndex];
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
            for (var i = columnIndex + 1; i < dataTable.Columns.Count; i++)
            {
                var dataColumn = dataTable.Columns[i];
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
            var dataRow = dataTable.DefaultView[rowIndex].Row;

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

                if ((columnIndex + 1) % columnCount != 0)
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