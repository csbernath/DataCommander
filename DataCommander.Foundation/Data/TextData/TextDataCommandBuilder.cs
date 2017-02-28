namespace DataCommander.Foundation.Data
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// 
    /// </summary>
    public static class TextDataCommandBuilder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="converters"></param>
        /// <param name="rows"></param>
        /// <param name="getTextWriter"></param>
        /// <returns></returns>
        public static TextDataCommand GetInsertCommand(
            IList<TextDataColumn> columns,
            IList<ITextDataConverter> converters,
            IEnumerable<object[]> rows,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="converters"></param>
        /// <param name="getTextReader"></param>
        /// <returns></returns>
        public static TextDataCommand GetSelectCommand(
            IList<TextDataColumn> columns,
            IList<ITextDataConverter> converters,
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
}