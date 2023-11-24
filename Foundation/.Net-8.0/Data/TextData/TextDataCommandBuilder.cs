using System.Collections.Generic;
using System.IO;

namespace Foundation.Data.TextData;

public static class TextDataCommandBuilder
{
    public static TextDataCommand GetInsertCommand(IList<TextDataColumn> columns, IList<ITextDataConverter> converters, IEnumerable<object[]> rows,
        IConverter<TextDataCommand, TextWriter> getTextWriter)
    {
        var command = new TextDataCommand();
        var parameters = command.Parameters;
        parameters.Add(new TextDataParameter("columns", columns));
        parameters.Add(new TextDataParameter("converters", converters));
        parameters.Add(new TextDataParameter("rows", rows));
        parameters.Add(new TextDataParameter("getTextWriter", getTextWriter));
        return command;
    }

    public static TextDataCommand GetSelectCommand(IList<TextDataColumn> columns, IList<ITextDataConverter> converters,
        IConverter<TextDataCommand, TextReader> getTextReader)
    {
        var command = new TextDataCommand();
        var parameters = command.Parameters;
        parameters.Add(new TextDataParameter("columns", columns));
        parameters.Add(new TextDataParameter("converters", converters));
        parameters.Add(new TextDataParameter("getTextReader", getTextReader));
        return command;
    }
}