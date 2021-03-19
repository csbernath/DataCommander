using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using DataCommander.Providers2.ResultWriter;

namespace DataCommander.Providers.ResultWriter
{
    internal static class HtmlFormatter
    {
        public static void Write(DataView dataView, int[] columnIndexes, TextWriter textWriter)
        {
            var htmlTable = new HtmlTable();
            htmlTable.Border = 1;
            htmlTable.CellPadding = 1;
            htmlTable.CellSpacing = 0;
            //Font font = DataCommander.Providers.Application.Instance.MainForm.SelectedFont;
            //htmlTable.Style["font-family"] = font.FontFamily.Name;
            //htmlTable.Style["font-size"] = font.SizeInPoints.ToString(CultureInfo.InvariantCulture) + "pt";

            htmlTable.Style["font-family"] = "Tahoma";
            htmlTable.Style["font-size"] = "8pt";

            var row = new HtmlTableRow();
            var dataTable = dataView.Table;
            var columns = dataTable.Columns;

            for (var i = 0; i < columnIndexes.Length; i++)
            {
                var cell = new HtmlTableCell("TH");
                var columnIndex = columnIndexes[i];
                var column = columns[columnIndex];
                var columnName = column.ColumnName;
                string html;

                if (columnName.Length > 0)
                {
                    columnName = columnName.Trim();
                }

                html = !string.IsNullOrEmpty(columnName)
                    ? HttpUtility.HtmlEncode(column.ColumnName)
                    : HtmlEntity.NonBreakingSpace;

                html = "<nobr><handled>" + html + "</handled></nobr>";
                cell.InnerHtml = html;
                row.Cells.Add(cell);
            }

            htmlTable.Rows.Add(row);

            var count = dataTable.Columns.Count;

            foreach (DataRowView dataRowView in dataView)
            {
                row = new HtmlTableRow();

                for (var i = 0; i < columnIndexes.Length; i++)
                {
                    var cell = new HtmlTableCell();
                    var columnIndex = columnIndexes[i];
                    var dataColumn = columns[columnIndex];
                    var type = (Type)dataColumn.ExtendedProperties[0];
                    if (type == null)
                    {
                        type = (Type)dataColumn.DataType;
                    }

                    var typeCode = Type.GetTypeCode(type);

                    if (typeCode == TypeCode.Byte ||
                        typeCode == TypeCode.SByte ||
                        typeCode == TypeCode.Int16 ||
                        typeCode == TypeCode.Int32 ||
                        typeCode == TypeCode.Decimal ||
                        typeCode == TypeCode.Single ||
                        typeCode == TypeCode.Double)
                    {
                        cell.Align = "right";
                    }

                    var value = dataRowView[i];
                    string valueStr;

                    if (value == DBNull.Value)
                    {
                        valueStr = "(null)";
                    }
                    else
                    {
                        valueStr = value.ToString().Trim();

                        if (valueStr.Length > 0)
                        {
                            if (valueStr.IndexOf('\r') >= 0 || valueStr.IndexOf('\n') >= 0)
                            {
                                var sb = new StringBuilder();
                                sb.Append("<pre>");
                                var s = HttpUtility.HtmlEncode(valueStr);
                                sb.Append(s);
                                sb.Append("</pre>");

                                valueStr = sb.ToString();
                            }
                            else
                            {
                                valueStr = HttpUtility.HtmlEncode(valueStr);
                                valueStr = "<nobr>" + valueStr + "</nobr>";
                            }
                        }
                        else
                        {
                            valueStr = HtmlEntity.NonBreakingSpace;
                        }
                    }

                    cell.InnerHtml = valueStr;

                    row.Cells.Add(cell);
                }

                htmlTable.Rows.Add(row);
            }

            var htmlTextWriter = new HtmlTextWriter(textWriter);
            htmlTable.RenderControl(htmlTextWriter);
        }
    }
}