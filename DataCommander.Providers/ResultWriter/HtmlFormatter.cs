namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;

    /// <summary>
    /// Summary description for HtmlFormatter.
    /// </summary>
    internal static class HtmlFormatter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="columnIndexes"></param>
        /// <param name="textWriter"></param>
        public static void Write(
            DataView dataView,
            int[] columnIndexes,
            TextWriter textWriter)
        {
            HtmlTable htmlTable = new HtmlTable();
            htmlTable.Border = 1;
            htmlTable.CellPadding = 1;
            htmlTable.CellSpacing = 0;
            //Font font = DataCommander.Providers.Application.Instance.MainForm.SelectedFont;
            //htmlTable.Style["font-family"] = font.FontFamily.Name;
            //htmlTable.Style["font-size"] = font.SizeInPoints.ToString(CultureInfo.InvariantCulture) + "pt";

            htmlTable.Style["font-family"] = "Tahoma";
            htmlTable.Style["font-size"] = "8pt";

            var row = new HtmlTableRow();
            DataTable dataTable = dataView.Table;
            var columns = dataTable.Columns;

            for (int i = 0; i < columnIndexes.Length; i++)
            {
                var cell = new HtmlTableCell("TH");
                int columnIndex = columnIndexes[i];
                var column = columns[columnIndex];
                string columnName = column.ColumnName;
                string html;

                if (columnName.Length > 0)
                {
                    columnName = columnName.Trim();
                }

                if (!string.IsNullOrEmpty(columnName))
                {
                    html = HttpUtility.HtmlEncode(column.ColumnName);
                }
                else
                {
                    html = HtmlEntity.NonBreakingSpace;
                }

                html = "<nobr><handled>" + html + "</handled></nobr>";
                cell.InnerHtml = html;
                row.Cells.Add(cell);
            }

            htmlTable.Rows.Add(row);

            int count = dataTable.Columns.Count;

            foreach (DataRowView dataRowView in dataView)
            {
                row = new HtmlTableRow();

                for (int i = 0; i < columnIndexes.Length; i++)
                {
                    var cell = new HtmlTableCell();
                    int columnIndex = columnIndexes[i];
                    var dataColumn = columns[columnIndex];
                    Type type = (Type)dataColumn.ExtendedProperties[0];
                    if (type == null)
                    {
                        type = (Type)dataColumn.DataType;
                    }

                    TypeCode typeCode = Type.GetTypeCode(type);

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

                    object value = dataRowView[i];
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
                                string s = HttpUtility.HtmlEncode(valueStr);
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