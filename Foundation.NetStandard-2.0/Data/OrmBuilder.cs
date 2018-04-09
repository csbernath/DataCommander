using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Foundation.Data
{
    public sealed class OrmBuilder
    {
        private readonly string _commandText;
        private readonly string _namespace;
        private readonly OrmQuery _query;
        private readonly ReadOnlyCollection<OrmResult> _results;

        public OrmBuilder(string commandText, string @namespace, OrmQuery query, ReadOnlyCollection<OrmResult> results)
        {
            _commandText = commandText;
            _namespace = @namespace;
            _query = query;
            _results = results;
        }

        public string ToString(bool properties)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($@"using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading;
using Foundation.Assertions;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace {_namespace}
{{
");

            stringBuilder.Append(GetQueryClass().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetQueryResultClass().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetRecordClasses(properties).Indent(1));
            stringBuilder.Append(GetHandlerClass().Indent(1).Replace("\"\"", $"\"{_commandText}\""));
            stringBuilder.Append("\r\n}");

            return stringBuilder.ToString();
        }

        private string GetHandlerClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"\r\n\r\npublic class {_query.Name}Handler\r\n{{\r\n");
            stringBuilder.Append("    private const string CommandText = @\"\";\r\n");
            stringBuilder.Append("    private readonly IDbConnection _connection;\r\n");
            stringBuilder.Append("    private readonly IDbTransaction _transaction;\r\n\r\n");
            stringBuilder.Append($"    public {_query.Name}Handler(IDbConnection connection, IDbTransaction transaction)\r\n");
            stringBuilder.Append("    {\r\n");
            stringBuilder.Append("        Assert.IsNotNull(connection);\r\n");
            stringBuilder.Append("        _connection = connection;\r\n");
            stringBuilder.Append("        _transaction = transaction;\r\n");
            stringBuilder.Append("    }\r\n\r\n");
            stringBuilder.Append(GetHandleQueryMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetHandleRequestMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");

            var sequence = new Sequence();
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

        private static string ToUpper(string camelCase) => char.ToUpper(camelCase[0]) + camelCase.Substring(1);

        private string GetHandleQueryMethod()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($@"public {_query.Name}QueryResult Handle({_query.Name}Query query)
{{
    Assert.IsNotNull(query);
    var parameters = new SqlParameterCollectionBuilder();
");
            foreach (var parameter in _query.Parameters)
            {
                stringBuilder.Append($"    parameters.Add({parameter.SqlParameterName}");

                if (parameter.SqlDbType != null)
                    stringBuilder.Append($", {parameter.SqlDbType}");

                stringBuilder.Append($", query.{ToUpper(parameter.SqlParameterName)});\r\n");
            }

            stringBuilder.Append($@"
    const int commandTimeout = 0;
    var createCommandRequest = new CreateCommandRequest(CommandText, parameters.ToReadOnlyCollection(), CommandType.Text, commandTimeout, _transaction);
    var executeReaderRequest = new ExecuteReaderRequest(createCommandRequest, CommandBehavior.Default, CancellationToken.None);

    return Handle(executeReaderRequest);
}}");

            return stringBuilder.ToString();
        }

        private string GetHandleRequestMethod()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($@"private {_query.Name}QueryResult Handle(ExecuteReaderRequest request)
{{
    {_query.Name}QueryResult result = null;
    var executor = _connection.CreateCommandExecutor();
    executor.ExecuteReader(request, dataReader =>
    {{
");

            var sequence = new Sequence();
            foreach (var result in _results)
            {
                var index = sequence.Next();
                string next = null;
                if (index > 0)
                {
                    stringBuilder.Append("\r\n");
                    next = "Next";
                }

                stringBuilder.Append($"        var result{index + 1} = dataReader.Read{next}Result(Read{result.RecordClassName}).AsReadOnly();");
            }

            stringBuilder.Append($"\r\n        result = new {_query.Name}QueryResult(");

            sequence.Reset();
            foreach (var result in _results)
            {
                var index = sequence.Next();
                if (index > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append($"result{index + 1}");
            }

            stringBuilder.Append(");\r\n");
            stringBuilder.Append("    });\r\n\r\n");
            stringBuilder.Append("    return result;\r\n");
            stringBuilder.Append("}");
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

        private string GetQueryClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"public class {_query.Name}Query
{{
");
            foreach (var parameter in _query.Parameters)
                stringBuilder.Append($"    public readonly {parameter.CSharpTypeName} {ToUpper(parameter.SqlParameterName)};\r\n");

            stringBuilder.Append("\r\n");
            stringBuilder.Append(GetQueryClassConstructor().Indent(1));
            stringBuilder.Append("\r\n}");
            return stringBuilder.ToString();
        }

        private string GetQueryClassConstructor()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"public {_query.Name}Query(");

            var sequence = new Sequence();
            foreach (var parameter in _query.Parameters)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append($"{parameter.CSharpTypeName} {parameter.SqlParameterName}");
            }

            stringBuilder.Append(")\r\n{\r\n");

            foreach (var parameter in _query.Parameters)
                stringBuilder.Append($"    {ToUpper(parameter.SqlParameterName)} = {parameter.SqlParameterName};\r\n");

            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        private string GetQueryResultClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"public class {_query.Name}QueryResult\r\n{{\r\n");

            var sequence = new Sequence();
            foreach (var result in _results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n");

                stringBuilder.Append($"    public readonly ReadOnlyCollection<{result.RecordClassName}> {result.RecordClassName};");
            }

            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetQueryResultClassConstructor());
            stringBuilder.Append("\r\n}");
            return stringBuilder.ToString();
        }

        private string GetQueryResultClassConstructor()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"    public {_query.Name}QueryResult(");

            var sequence = new Sequence();
            foreach (var result in _results)
            {
                var index = sequence.Next();
                if (index > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append($"ReadOnlyCollection<{result.RecordClassName}> result{index + 1}");
            }

            stringBuilder.Append(")\r\n");
            stringBuilder.Append("    {\r\n");

            sequence.Reset();
            foreach (var result in _results)
            {
                var index = sequence.Next();
                if (index > 0)
                    stringBuilder.Append("\r\n");

                stringBuilder.Append($"        {result.RecordClassName} = result{index + 1};");
            }

            stringBuilder.Append("\r\n    }");

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

    public sealed class OrmQuery
    {
        public readonly string Name;
        public readonly ReadOnlyCollection<OrmParameter> Parameters;

        public OrmQuery(string name, ReadOnlyCollection<OrmParameter> parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }
}