using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Data;

namespace DataCommander.Application.ResultWriter;

public class HtmlResultWriter(Action<InfoMessage> addInfoMessage) : IResultWriter
{
    private readonly IResultWriter _logResultWriter = new LogResultWriter(addInfoMessage, false);
    private HtmlTextWriter? _htmlTextWriter;

    void IResultWriter.AfterCloseReader(int affectedRows) => _logResultWriter.AfterCloseReader(affectedRows);
    void IResultWriter.AfterExecuteReader() => _logResultWriter.AfterExecuteReader();

    void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand) =>
        _logResultWriter.BeforeExecuteReader(asyncDataAdapterCommand);

    void IResultWriter.Begin(IProvider provider) => _logResultWriter.Begin(provider);
    void IResultWriter.End() => _logResultWriter.End();
    void IResultWriter.FirstRowReadBegin() => _logResultWriter.FirstRowReadBegin();
    void IResultWriter.FirstRowReadEnd(string[] dataTypeNames) => _logResultWriter.FirstRowReadEnd(dataTypeNames);
    void IResultWriter.WriteParameters(IDataParameterCollection parameters) => _logResultWriter.WriteParameters(parameters);

    void IResultWriter.WriteRows(object[][] rows, int rowCount)
    {
        _logResultWriter.WriteRows(rows, rowCount);

        for (var rowIndex = 0; rowIndex < rowCount; ++rowIndex)
        {
            var row = rows[rowIndex];

            _htmlTextWriter!.WriteLine();
            _htmlTextWriter.WriteFullBeginTag("tr");
            _htmlTextWriter.WriteLine();
            ++_htmlTextWriter.Indent;

            for (var columnIndex = 0; columnIndex < row.Length; ++columnIndex)
            {
                var value = row[columnIndex];

                if (columnIndex > 0)
                    _htmlTextWriter.WriteLine();

                _htmlTextWriter.WriteFullBeginTag("td");
                _htmlTextWriter.WriteEncodedText(value.ToString());
                _htmlTextWriter.WriteEndTag("td");
            }

            _htmlTextWriter.WriteLine();
            --_htmlTextWriter.Indent;
            _htmlTextWriter.WriteEndTag("tr");
        }
    }

    void IResultWriter.WriteTableBegin(DataTable schemaTable)
    {
        _logResultWriter.WriteTableBegin(schemaTable);

        var path = Path.GetTempFileName() + ".html";
        var streamWriter = new StreamWriter(path, false, Encoding.UTF8);
        _htmlTextWriter = new HtmlTextWriter(streamWriter);

        _htmlTextWriter.WriteFullBeginTag("table");
        _htmlTextWriter.WriteLine();
        ++_htmlTextWriter.Indent;

        var columns = schemaTable.Rows.Cast<DataRow>().Select(FoundationDbColumnFactory.Create).ToList();

        _htmlTextWriter.WriteFullBeginTag("tr");
        _htmlTextWriter.WriteLine();
        ++_htmlTextWriter.Indent;

        for (var columnIndex = 0; columnIndex < columns.Count; ++columnIndex)
        {
            var column = columns[columnIndex];

            if (columnIndex > 0)
                _htmlTextWriter.WriteLine();

            _htmlTextWriter.WriteFullBeginTag("th");
            _htmlTextWriter.WriteEncodedText(column.ColumnName);
            _htmlTextWriter.WriteEndTag("th");
        }

        _htmlTextWriter.WriteLine();
        --_htmlTextWriter.Indent;
        _htmlTextWriter.WriteEndTag("tr");
    }

    void IResultWriter.WriteTableEnd()
    {
        _logResultWriter.WriteTableEnd();

        --_htmlTextWriter!.Indent;
        _htmlTextWriter.WriteLine();
        _htmlTextWriter.WriteEndTag("table");
        _htmlTextWriter.Close();
        _htmlTextWriter = null;
    }
}