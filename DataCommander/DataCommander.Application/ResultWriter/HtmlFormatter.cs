using System.Data;
using System.IO;
using System.Web.UI;

namespace DataCommander.Application.ResultWriter;

internal static class HtmlFormatter
{
    public static void Write(DataView dataView, int[] columnIndexes, TextWriter textWriter)
    {
        var dataTable = dataView.Table!;
        var columns = dataTable.Columns;

        var htmlWriter = new HtmlTextWriter(textWriter);

        using (new HtmlTextWriterTagRenderer(htmlWriter, HtmlTextWriterTag.Style))
        {
            htmlWriter.WriteLine("table,th,td {border:1px solid black;border-spacing:0px}");
        }

        using (new HtmlTextWriterTagRenderer(htmlWriter, HtmlTextWriterTag.Table))
        {
            using (new HtmlTextWriterTagRenderer(htmlWriter, HtmlTextWriterTag.Tr))
            {
                for (var i = 0; i < columnIndexes.Length; ++i)
                {
                    using (new HtmlTextWriterTagRenderer(htmlWriter, HtmlTextWriterTag.Th))
                    {
                        var columnIndex = columnIndexes[i];
                        var column = columns[columnIndex];
                        var columnName = column.ColumnName;
                        htmlWriter.WriteEncodedText(columnName);
                    }
                }
            }
            
            foreach (DataRowView dataRowView in dataView)
            {
                using (new HtmlTextWriterTagRenderer(htmlWriter, HtmlTextWriterTag.Tr))
                {
                    for (var i = 0; i < columnIndexes.Length; ++i)
                    {
                        using (new HtmlTextWriterTagRenderer(htmlWriter, HtmlTextWriterTag.Td))
                        {
                            var value = dataRowView[i];
                            var valueString = value.ToString();
                            htmlWriter.WriteEncodedText(valueString);
                        }
                    }
                }
            }
        }
    }
}