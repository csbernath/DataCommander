using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Foundation.Data
{
    public sealed class OrmBuilder
    {
        private readonly string _commandText;
        private readonly ReadOnlyCollection<OrmResult> _results;

        public OrmBuilder(string commandText, ReadOnlyCollection<OrmResult> results)
        {
            _commandText = commandText;
            _results = results;
        }

        public string ToString(bool properties)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(GetSqlQueryResultClass());
            stringBuilder.Append(GetRecordClasses(properties));
            stringBuilder.Append(GetSqlQueryHandler());

            return stringBuilder.ToString();
        }

        private string GetSqlQueryHandler()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("\r\n\r\npublic class SqlQueryHandler\r\n{\r\n");
            stringBuilder.Append($"    private const string CommandText = @\"{_commandText}\";\r\n");
            stringBuilder.Append("    private readonly IDbConnection _connection;\r\n");
            stringBuilder.Append("    private readonly IDbTransaction _transaction;\r\n\r\n");
            stringBuilder.Append("    public SqlQueryHandler(IDbConnection connection, IDbTransaction transaction)\r\n");
            stringBuilder.Append("    {\r\n");
            stringBuilder.Append("        Assert.IsNotNull(_connection);\r\n");
            stringBuilder.Append("        _connection = connection;\r\n");
            stringBuilder.Append("        _transaction = transaction;\r\n");
            stringBuilder.Append("    }\r\n\r\n");
            stringBuilder.Append("    public SqlQueryResult Handle()\r\n    {\r\n");
            stringBuilder.Append("        var executor = _connection.CreateCommandExecutor();\r\n");
            stringBuilder.Append("        SqlQueryResult sqlQueryResult = null;\r\n");
            stringBuilder.Append("        executor.ExecuteReader(new ExecuteReaderRequest(CommandText, null, _transaction), dataReader =>\r\n");
            stringBuilder.Append("        {\r\n");

            var sequence = new Sequence();
            foreach (var result in _results)
            {
                var index = sequence.Next();
                if (index > 0)
                    stringBuilder.Append("\r\n");

                stringBuilder.Append($"            var result{index} = dataReader.ReadResult(Read{result.RecordClassName}).AsReadOnly();");
            }

            stringBuilder.Append("\r\n            sqlQueryResult = new SqlQueryResult(");

            sequence.Reset();
            foreach (var result in _results)
            {
                var index = sequence.Next();
                if (index > 0)
                    stringBuilder.Append(',');

                stringBuilder.Append($"result{index}");
            }

            stringBuilder.Append(");\r\n");
            stringBuilder.Append("        });\r\n\r\n");
            stringBuilder.Append("        return sqlQueryResult;");
            stringBuilder.Append("    }\r\n\r\n");

            sequence.Reset();
            foreach (var result in _results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n\r\n");

                var readMethod = GetReadRecordMethod(result);
                stringBuilder.Append(readMethod.Indent(1));
            }

            stringBuilder.Append("\r\n}");
            return stringBuilder.ToString();
        }

        private string GetRecordClasses(bool properties)
        {
            var stringBuilder = new StringBuilder();
            var sequence = new Sequence();
            foreach (var result in _results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n\r\n");

                var recordClass = GetRecordClass(result, properties);
                stringBuilder.Append(recordClass);
            }

            return stringBuilder.ToString();
        }

        private string GetSqlQueryResultClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("public class SqlQueryResult\r\n{\r\n");

            var sequence = new Sequence();
            foreach (var result in _results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n");

                stringBuilder.Append($"    public readonly ReadOnlyCollection<{result.RecordClassName}> {result.RecordClassName};");
            }

            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetSqlQueryResultClassConstructor());
            stringBuilder.Append("\r\n}\r\n\r\n");
            return stringBuilder.ToString();
        }

        private string GetSqlQueryResultClassConstructor()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"    public SqlQueryResult(");

            var sequence = new Sequence();
            foreach (var result in _results)
            {
                var index = sequence.Next();
                if (index > 0)
                    stringBuilder.Append(',');

                stringBuilder.Append($"ReadOnlyCollection<{result.RecordClassName}> result{index}");
            }

            stringBuilder.Append(")\r\n");
            stringBuilder.Append("    {\r\n");
            stringBuilder.Append("    }");

            sequence.Reset();
            foreach (var result in _results)
            {
                var index = sequence.Next();
                if (index > 0)
                    stringBuilder.Append("\r\n");

                stringBuilder.Append($"{result.RecordClassName} = result{index};");
            }

            stringBuilder.Append("\r\n}");

            return stringBuilder.ToString();
        }

        private static string GetRecordClass(OrmResult result, bool properties)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("public class ");
            stringBuilder.Append(result.RecordClassName);
            stringBuilder.Append("\r\n{\r\n");

            var sequence = new Sequence();
            foreach (var column in result.Columns)
            {
                if (sequence.Next() > 0)
                    stringBuilder.AppendLine();

                stringBuilder.Append("    public ");
                stringBuilder.Append(GetCSharpTypeName(column.DataType));

                if (column.AllowDbNull && IsValueType(column.DataType))
                    stringBuilder.Append('?');

                stringBuilder.Append(' ');
                stringBuilder.Append(column.ColumnName);
                stringBuilder.Append(properties ? " { get; set; }" : ";");
            }

            stringBuilder.Append(@"
}");
            return stringBuilder.ToString();
        }

        private static string GetReadRecordMethod(OrmResult result)
        {
            const string recordInstanceName = "record";

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendFormat(@"private static {0} Read{0}(IDataRecord dataRecord)
{{
    var {1} = new {0}();
", result.RecordClassName, recordInstanceName);

            var first = true;
            var index = 0;
            foreach (var column in result.Columns)
            {
                if (first)
                    first = false;
                else
                    stringBuilder.AppendLine();

                stringBuilder.AppendFormat("    {0}.", recordInstanceName);
                stringBuilder.Append(column.ColumnName);
                stringBuilder.Append(" = dataRecord.");
                stringBuilder.Append(GetDataRecordMethodName(column));
                stringBuilder.Append('(');
                stringBuilder.Append(index);
                stringBuilder.Append(");");

                ++index;
            }

            stringBuilder.AppendFormat(@"
    return {0};
}}", recordInstanceName);

            return stringBuilder.ToString();
        }

        private static string GetCSharpTypeName(Type dbColumnDataType)
        {
            var typeCode = Type.GetTypeCode(dbColumnDataType);
            string csharpTypeName;
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    csharpTypeName = CSharpTypeName.Boolean;
                    break;
                case TypeCode.Char:
                    csharpTypeName = CSharpTypeName.Char;
                    break;
                case TypeCode.SByte:
                    csharpTypeName = CSharpTypeName.SByte;
                    break;
                case TypeCode.Byte:
                    csharpTypeName = CSharpTypeName.Byte;
                    break;
                case TypeCode.Int16:
                    csharpTypeName = CSharpTypeName.Int16;
                    break;
                case TypeCode.UInt16:
                    csharpTypeName = CSharpTypeName.UInt16;
                    break;
                case TypeCode.Int32:
                    csharpTypeName = CSharpTypeName.Int32;
                    break;
                case TypeCode.UInt32:
                    csharpTypeName = CSharpTypeName.UInt32;
                    break;
                case TypeCode.Int64:
                    csharpTypeName = CSharpTypeName.Int64;
                    break;
                case TypeCode.UInt64:
                    csharpTypeName = CSharpTypeName.UInt64;
                    break;
                case TypeCode.Single:
                    csharpTypeName = CSharpTypeName.Single;
                    break;
                case TypeCode.Double:
                    csharpTypeName = CSharpTypeName.Double;
                    break;
                case TypeCode.Decimal:
                    csharpTypeName = CSharpTypeName.Decimal;
                    break;
                case TypeCode.String:
                    csharpTypeName = CSharpTypeName.String;
                    break;
                case TypeCode.DateTime:
                    csharpTypeName = nameof(DateTime);
                    break;
                default:
                    if (dbColumnDataType == typeof(byte[]))
                        csharpTypeName = "byte[]";
                    else
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

                case TypeCode.Object:
                    if (dbColumnDataType == typeof(Guid))
                        isValueType = true;
                    break;
            }

            return isValueType;
        }

        private static string GetDataRecordMethodName(OrmColumn column)
        {
            var typeCode = Type.GetTypeCode(column.DataType);
            string methodName = null;
            switch (typeCode)
            {
                case TypeCode.Empty:
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
                case TypeCode.Object:
                    if (column.DataType == typeof(Guid))
                        methodName = column.AllowDbNull == true ? "GetNullableGuid" : "GetGuid";
                    else if (column.DataType == typeof(byte[]))
                        methodName = column.AllowDbNull == true ? "GetNullableBytes" : "GetBytes";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return methodName;
        }
    }
}