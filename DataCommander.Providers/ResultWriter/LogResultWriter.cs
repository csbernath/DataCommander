using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DataCommander.Providers.Connection;
using Foundation;
using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Log;
using Foundation.Linq;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class LogResultWriter : IResultWriter
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly Action<InfoMessage> _addInfoMessage;
        private int _commandCount;
        private int _tableCount;
        private int _rowCount;
        private long _beginTimestamp;
        private long _beforeExecuteReaderTimestamp;
        private long _writeTableBeginTimestamp;
        private long _firstRowReadBeginTimestamp;

        public LogResultWriter(Action<InfoMessage> addInfoMessage)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(addInfoMessage != null);
#endif
            _addInfoMessage = addInfoMessage;
        }

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider)
        {
            _beginTimestamp = Stopwatch.GetTimestamp();
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand asyncDataAdapterCommand)
        {
            _beforeExecuteReaderTimestamp = Stopwatch.GetTimestamp();
            var command = asyncDataAdapterCommand.Command;
            var message = $"Executing command[{_commandCount}] from line {asyncDataAdapterCommand.LineIndex + 1}...\r\n{command.CommandText}";

            ++_commandCount;

            var parameters = command.Parameters;
            if (!parameters.IsNullOrEmpty())
            {
                message += "\r\n" + command.Parameters.ToLogString();
            }

            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            var duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
            var message = $"Command[{_commandCount - 1}] started in {StopwatchTimeSpan.ToString(duration, 3)} seconds. Field count: {fieldCount}";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
            _tableCount = 0;
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
            var duration = Stopwatch.GetTimestamp() - _beforeExecuteReaderTimestamp;
            var now = LocalTime.Default.Now;
            var affected = affectedRows >= 0
                ? $"{affectedRows} row(s) affected."
                : null;
            var message = $"Command[{_commandCount - 1}] completed in {StopwatchTimeSpan.ToString(duration, 3)} seconds. {affected}";
            _addInfoMessage(new InfoMessage(now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            _writeTableBeginTimestamp = Stopwatch.GetTimestamp();
            var columns = schemaTable.Rows.Cast<DataRow>().Select(i => new DbColumn(i)).ToList();

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("\r\ninternal sealed class Row");
            stringBuilder.Append(_tableCount);
            stringBuilder.Append("\r\n{\r\n");

            var first = true;
            foreach (var column in columns)
            {
                if (first)
                    first = false;
                else
                    stringBuilder.AppendLine();

                stringBuilder.Append("    public ");
                stringBuilder.Append(GetCSharpTypeName(column.DataType));

                if (column.AllowDbNull == true && IsValueType(column.DataType))
                    stringBuilder.Append('?');

                stringBuilder.Append(' ');
                stringBuilder.Append(column.ColumnName);
                stringBuilder.Append(";");
            }

            stringBuilder.Append("\r\n}\r\nprivate static Row");
            stringBuilder.Append(_tableCount);
            stringBuilder.Append(" Read");
            stringBuilder.Append(_tableCount);
            stringBuilder.Append("(IDataRecord dataRecord)\r\n{\r\n    var row = new Row");
            stringBuilder.Append(_tableCount);
            stringBuilder.Append("();\r\n");

            first = true;
            var index = 0;
            foreach (var column in columns)
            {
                if (first)
                    first = false;
                else
                    stringBuilder.AppendLine();

                stringBuilder.Append("    row.");
                stringBuilder.Append(column.ColumnName);
                stringBuilder.Append(" = dataRecord.");
                stringBuilder.Append(GetDataRecordMethodName(column));
                stringBuilder.Append('(');
                stringBuilder.Append(index);
                stringBuilder.Append(");");

                ++index;
            }

            stringBuilder.Append("\r\n    return row;\r\n}\r\n");

            Log.Trace($"SchemaTable of table[{_tableCount}]:\r\n{schemaTable.ToStringTableString()}\r\n{stringBuilder}");

            ++_tableCount;
            _rowCount = 0;
        }

        private static string GetDataRecordMethodName(DbColumn column)
        {
            var typeCode = Type.GetTypeCode(column.DataType);
            string methodName = null;
            switch (typeCode)
            {
                case TypeCode.Empty:
                    break;
                case TypeCode.Object:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    methodName = column.AllowDbNull == true ? "GetNullableBoolean" : "GetBoolean";
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.SByte:
                    break;
                case TypeCode.Byte:
                    methodName = column.AllowDbNull == true ? "GetNullableByte" : "GetByte";
                    break;
                case TypeCode.Int16:
                    methodName = column.AllowDbNull == true ? "GetNullableInt16" : "GetInt16";
                    break;
                case TypeCode.UInt16:
                    break;
                case TypeCode.Int32:
                    methodName = column.AllowDbNull == true ? "GetNullableInt32" : "GetInt32";
                    break;
                case TypeCode.UInt32:
                    break;
                case TypeCode.Int64:
                    methodName = column.AllowDbNull == true ? "GetNullableInt64" : "GetInt64";
                    break;
                case TypeCode.UInt64:
                    break;
                case TypeCode.Single:
                    methodName = column.AllowDbNull == true ? "GetNullableFloat" : "GetFloat";
                    break;
                case TypeCode.Double:
                    methodName = column.AllowDbNull == true ? "GetNullableDouble" : "GetDouble";
                    break;
                case TypeCode.Decimal:
                    methodName = column.AllowDbNull == true ? "GetNullableDecimal" : "GetDecimal";
                    break;
                case TypeCode.DateTime:
                    methodName = column.AllowDbNull == true ? "GetNullableDateTime" : "GetDateTime";
                    break;
                case TypeCode.String:
                    methodName = column.AllowDbNull == true ? "GetStringOrDefault" : "GetString";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return methodName;
        }

        private static string GetCSharpTypeName(Type dbColumnDataType)
        {
            var typeCode = Type.GetTypeCode(dbColumnDataType);
            string csharpTypeName;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    csharpTypeName = "bool";
                    break;
                case TypeCode.Char:
                    csharpTypeName = "char";
                    break;
                case TypeCode.SByte:
                    csharpTypeName = "byte";
                    break;
                case TypeCode.Byte:
                    csharpTypeName = "byte";
                    break;
                case TypeCode.Int16:
                    csharpTypeName = "shorrt";
                    break;
                case TypeCode.UInt16:
                    csharpTypeName = "ushort";
                    break;
                case TypeCode.Int32:
                    csharpTypeName = "int";
                    break;
                case TypeCode.UInt32:
                    csharpTypeName = "uint";
                    break;
                case TypeCode.Int64:
                    csharpTypeName = "long";
                    break;
                case TypeCode.UInt64:
                    csharpTypeName = "ulong";
                    break;
                case TypeCode.Single:
                    csharpTypeName = "float";
                    break;
                case TypeCode.Double:
                    csharpTypeName = "double";
                    break;
                case TypeCode.Decimal:
                    csharpTypeName = "decimal";
                    break;
                case TypeCode.String:
                    csharpTypeName = "string";
                    break;
                default:
                    csharpTypeName = dbColumnDataType.Name;
                    break;
            }
            return csharpTypeName;
        }

        private static bool IsValueType(Type dbColumnDataType)
        {
            var typeCode = Type.GetTypeCode(dbColumnDataType);
            var isValueType = false;

            switch (typeCode)
            {
                case TypeCode.Boolean:
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                    isValueType = true;
                    break;
            }

            return isValueType;
        }

        void IResultWriter.FirstRowReadBegin()
        {
            _firstRowReadBeginTimestamp = Stopwatch.GetTimestamp();
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
            var duration = Stopwatch.GetTimestamp() - _firstRowReadBeginTimestamp;
            var message = $"First row read completed in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            _rowCount += rowCount;
        }

        void IResultWriter.WriteTableEnd()
        {
            var duration = Stopwatch.GetTimestamp() - _writeTableBeginTimestamp;
            var message =
                $"Reading {_rowCount} row(s) from command[{_commandCount - 1}] into table[{_tableCount - 1}] finished in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            var duration = Stopwatch.GetTimestamp() - _beginTimestamp;
            var message = $"Query completed {_commandCount} command(s) in {StopwatchTimeSpan.ToString(duration, 3)} seconds.";
            _addInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Verbose, message));
        }

        #endregion
    }
}