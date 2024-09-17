using System;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Windows.Forms;
using DataCommander.Application.ResultWriter;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.FieldReaders;
using DataCommander.Api.Query;
using Foundation.Core;

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
        Control control = tableStyle switch
        {
            ResultWriterType.DataGrid => CreateDataTableEditorFromDataTable(queryForm, commandBuilder, dataTable, getTableSchemaResult, readOnly, colorTheme),
            //case ResultWriterType.Html:
            //    control = CreateHtmlTextBoxFromDataTable(dataTable);
            //    break;
            ResultWriterType.ListView => CreateListViewFromDataTable(dataTable),
            _ => throw new NotImplementedException(),
        };
        return control;
    }

    private static DataTableEditor CreateDataTableEditorFromDataTable(
        IQueryForm queryForm,
        DbCommandBuilder commandBuilder,
        DataTable dataTable,
        GetTableSchemaResult getTableSchemaResult,
        bool readOnly,
        ColorTheme colorTheme)
    {
        DataTableEditor editor = new DataTableEditor(queryForm, commandBuilder, colorTheme)
        {
            ReadOnly = readOnly,
            DataTable = dataTable,
            TableName = dataTable.TableName,
            TableSchema = getTableSchemaResult
        };
        return editor;
    }

    private static ListView CreateListViewFromDataTable(DataTable dataTable)
    {
        ListView listView = new ListView
        {
            View = View.Details,
            GridLines = true,
            AllowColumnReorder = true,
            Font = new Font("Tahoma", 8),
            FullRowSelect = true
        };

        foreach (DataColumn dataColumn in dataTable.Columns)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Text = dataColumn.ColumnName,
                Width = -2
            };

            Type? type = (Type)dataColumn.ExtendedProperties[0];

            if (type == null)
            {
                type = dataColumn.DataType;
            }

            columnHeader.TextAlign = GetHorizontalAlignment(type);

            listView.Columns.Add(columnHeader);
        }

        int count = dataTable.Columns.Count;
        string[] items = new string[count];

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

            ListViewItem listViewItem = new ListViewItem(items);
            listView.Items.Add(listViewItem);
        }

        return listView;
    }

    public static HorizontalAlignment GetHorizontalAlignment(Type type)
    {
        TypeCode typeCode = Type.GetTypeCode(type);
        HorizontalAlignment align = typeCode switch
        {
            TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Byte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Decimal => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Left,
        };
        return align;
    }

    public static bool FindText(DataView dataView, IStringMatcher matcher, ref int rowIndex, ref int columnIndex)
    {
        bool found = false;
        DataTable? dataTable = dataView.Table;
        int rowCount = dataView.Count;
        int columnCount = dataTable.Columns.Count;
        object currentValueObject = dataTable.DefaultView[rowIndex][columnIndex];
        string currentValue;
        if (currentValueObject is StringField)
        {
            StringField? stringField = currentValueObject as StringField;
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

    public static void AddInfoMessageToQueryForm(QueryForm queryForm, long elapsedTicks, string connectionName, string providerName,
        ConnectionBase connection)
    {
        string message = $@"Connection opened in {StopwatchTimeSpan.ToString(elapsedTicks, 3)} seconds.
Connection name: {connectionName}
Provider name:   {providerName}
Data source:     {connection.DataSource}
Database:        {connection.Database}
Server version:  {connection.ServerVersion}
{connection.ConnectionInformation}";

        InfoMessage infoMessage = InfoMessageFactory.Create(InfoMessageSeverity.Verbose, null, message);
        queryForm.AddInfoMessage(infoMessage);
    }
}