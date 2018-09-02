using System;
using System.Data;
using System.Linq;
using System.Text;
using Foundation.Assertions;
using Foundation.Collections.ReadOnly;
using Foundation.Data.SqlClient;

namespace Foundation.Data.DbQueryBuilding
{
    public sealed class DbRequestBuilder
    {
        private readonly DbRequest _request;

        public DbRequestBuilder(DbRequest request)
        {
            Assert.IsNotNull(request);
            _request = request;
        }

        public string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($@"using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
{_request.Using}

namespace {_request.Namespace}
{{
");

            stringBuilder.Append(GetRequestClass().Indent(1));

            if (_request.Results.Count > 0)
            {
                stringBuilder.Append("\r\n\r\n");
                stringBuilder.Append(GetQueryResultClass().Indent(1));
                stringBuilder.Append("\r\n\r\n");
                stringBuilder.Append(GetRecordClasses().Indent(1));
            }

            stringBuilder.Append(GetHandlerClass().Indent(1).Replace("\"\"", $"\"{_request.CommandText}\""));
            stringBuilder.Append("\r\n}");

            return stringBuilder.ToString();
        }

        private static string GetCSharpTypeName(SqlDbType sqlDbType, string dataType, bool isNullable)
        {
            string csharpTypeName;
            if (sqlDbType == SqlDbType.Structured)
            {
                var userDefinedTableType = dataType.Split('.')[1];
                csharpTypeName = $"ReadOnlyList<{userDefinedTableType}>";
            }
            else
            {
                csharpTypeName = SqlDataTypeArray.SqlDataTypes.First(i => i.SqlDbType == sqlDbType).CSharpTypeName;
                if (isNullable)
                    csharpTypeName += "?";
            }

            return csharpTypeName;
        }

        private static string GetCSharpTypeName(Type dbColumnDataType, bool isNullable)
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

            if (isNullable && IsValueType(dbColumnDataType))
                csharpTypeName += '?';

            return csharpTypeName;
        }

        private static string GetDataRecordMethodName(DbQueryResultField field)
        {
            var typeCode = Type.GetTypeCode(field.DataType);
            string methodName = null;
            switch (typeCode)
            {
                case TypeCode.Empty:
                    break;
                case TypeCode.DBNull:
                    break;
                case TypeCode.Boolean:
                    methodName = field.IsNullable ? "GetNullableBoolean" : "GetBoolean";
                    break;
                case TypeCode.Char:
                    break;
                case TypeCode.SByte:
                    break;
                case TypeCode.Byte:
                    methodName = field.IsNullable ? "GetNullableByte" : "GetByte";
                    break;
                case TypeCode.Int16:
                    methodName = field.IsNullable ? "GetNullableInt16" : "GetInt16";
                    break;
                case TypeCode.UInt16:
                    break;
                case TypeCode.Int32:
                    methodName = field.IsNullable ? "GetNullableInt32" : "GetInt32";
                    break;
                case TypeCode.UInt32:
                    break;
                case TypeCode.Int64:
                    methodName = field.IsNullable ? "GetNullableInt64" : "GetInt64";
                    break;
                case TypeCode.UInt64:
                    break;
                case TypeCode.Single:
                    methodName = field.IsNullable ? "GetNullableFloat" : "GetFloat";
                    break;
                case TypeCode.Double:
                    methodName = field.IsNullable ? "GetNullableDouble" : "GetDouble";
                    break;
                case TypeCode.Decimal:
                    methodName = field.IsNullable ? "GetNullableDecimal" : "GetDecimal";
                    break;
                case TypeCode.DateTime:
                    methodName = field.IsNullable ? "GetNullableDateTime" : "GetDateTime";
                    break;
                case TypeCode.String:
                    methodName = field.IsNullable ? "GetStringOrDefault" : "GetString";
                    break;
                case TypeCode.Object:
                    if (field.DataType == typeof(Guid))
                        methodName = field.IsNullable ? "GetNullableGuid" : "GetGuid";
                    else if (field.DataType == typeof(byte[]))
                        methodName = field.IsNullable ? "GetNullableBytes" : "GetBytes";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return methodName;
        }

        private string GetHandlerClass()
        {
            var commandTimeoutString = _request.CommandTimeout != null ? _request.CommandTimeout.Value.ToString() : "null";

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"\r\n\r\npublic sealed class {_request.Name}Db{GetRequestType()}Handler\r\n{{\r\n");
            stringBuilder.Append("    private const string CommandText = @\"\";\r\n");
            stringBuilder.Append($"    private static int? CommandTimeout = {commandTimeoutString};\r\n");
            stringBuilder.Append("    private readonly IDbConnection _connection;\r\n");
            stringBuilder.Append("    private readonly IDbTransaction _transaction;\r\n\r\n");
            stringBuilder.Append($"    public {_request.Name}Db{GetRequestType()}Handler(IDbConnection connection, IDbTransaction transaction)\r\n");
            stringBuilder.Append("    {\r\n");
            stringBuilder.Append("        Assert.IsNotNull(connection);\r\n");
            stringBuilder.Append("        _connection = connection;\r\n");
            stringBuilder.Append("        _transaction = transaction;\r\n");
            stringBuilder.Append("    }\r\n\r\n");
            stringBuilder.Append(GetHandleMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetHandleAsyncMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetToCreateCommandRequestMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetToParametersMethod().Indent(1));

            if (_request.Results.Count > 0)
            {
                stringBuilder.Append("\r\n\r\n");
                stringBuilder.Append(GetToExecuteReaderRequestMethod().Indent(1));
                stringBuilder.Append("\r\n\r\n");
                stringBuilder.Append(GetExecuteReaderMethod().Indent(1));
                stringBuilder.Append("\r\n\r\n");
                stringBuilder.Append(GetExecuteReaderAsyncMethod().Indent(1));
            }

            stringBuilder.Append("\r\n\r\n");

            var sequence = new Sequence();
            foreach (var result in _request.Results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n\r\n");

                var readMethod = GetReadRecordMethod(result);
                stringBuilder.Append(readMethod.Indent(1));
            }

            stringBuilder.Append("\r\n}");
            return stringBuilder.ToString();
        }

        private string GetExecuteReaderAsyncMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"private async Task<{_request.Name}DbQueryResult> ExecuteReaderAsync(ExecuteReaderRequest request)
{{
    {_request.Name}DbQueryResult result = null;
    var connection = (DbConnection)_connection;
    var executor = connection.CreateCommandAsyncExecutor();
    await executor.ExecuteReaderAsync(request, async dataReader =>
    {{
{GetExecuteReaderAsyncMethodFragment().Indent(2)}
    }});

    return result;
}}");
            return stringBuilder.ToString();
        }

        private string GetExecuteReaderMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"private {_request.Name}DbQueryResult ExecuteReader(ExecuteReaderRequest request)
{{
    {_request.Name}DbQueryResult result = null;
    var executor = _connection.CreateCommandExecutor();
    executor.ExecuteReader(request, dataReader =>
    {{
");
            var sequence = new Sequence();
            foreach (var result in _request.Results)
            {
                var index = sequence.Next();
                string next = null;
                if (index > 0)
                {
                    stringBuilder.Append("\r\n");
                    next = "Next";
                }

                stringBuilder.Append($"        var {ToLower(result.FieldName)} = dataReader.Read{next}Result(Read{result.Name}).ToReadOnlyList();");
            }

            stringBuilder.Append($"\r\n        result = new {_request.Name}DbQueryResult(");

            sequence.Reset();
            foreach (var result in _request.Results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append($"{ToLower(result.FieldName)}");
            }

            stringBuilder.Append(");\r\n");
            stringBuilder.Append("    });\r\n\r\n");
            stringBuilder.Append("    return result;\r\n");
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

        private string GetHandleMethod()
        {
            var responseType = _request.Results.Count == 0 ? "int" : $"{_request.Name}DbQueryResult";
            var requestParameter = _request.Results.Count == 0 ? "command" : "query";

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"public {responseType} Handle({_request.Name}Db{GetRequestType()} {requestParameter})
{{
    Assert.IsNotNull({requestParameter});
");

            if (_request.Results.Count == 0)
            {
                stringBuilder.Append($@"    var request = ToCreateCommandRequest({ToLower(GetRequestType())});
    var executor = _connection.CreateCommandExecutor();
    return executor.ExecuteNonQuery(request);");
            }
            else
                stringBuilder.Append(@"    var request = ToExecuteReaderRequest(query, CancellationToken.None);
    return ExecuteReader(request);");

            stringBuilder.Append(@"
}");
            return stringBuilder.ToString();
        }

        private string GetHandleAsyncMethod()
        {
            var request = _request.Results.Count == 0 ? "command" : "query";
            var result = _request.Results.Count == 0 ? "int" : $"{_request.Name}DbQueryResult";
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"public Task<{result}> HandleAsync({_request.Name}Db{GetRequestType()} {request}, CancellationToken cancellationToken)
{{
    Assert.IsNotNull({request});
");

            if (_request.Results.Count == 0)
                stringBuilder.Append(@"    var createCommandRequest = ToCreateCommandRequest(command);
    var executeNonReaderRequest = new ExecuteNonReaderRequest(createCommandRequest, cancellationToken);
    var dbConnection = (DbConnection)_connection;
    var executor = dbConnection.CreateCommandAsyncExecutor();
    return executor.ExecuteNonQueryAsync(executeNonReaderRequest);");
            else
                stringBuilder.Append(@"    var request = ToExecuteReaderRequest(query, cancellationToken);
    return ExecuteReaderAsync(request);");

            stringBuilder.Append(@"
}");

            return stringBuilder.ToString();
        }

        private string GetExecuteReaderAsyncMethodFragment()
        {
            var stringBuilder = new StringBuilder();
            var sequence = new Sequence();
            foreach (var result in _request.Results)
            {
                var next = sequence.Next() == 0 ? null : "Next";
                stringBuilder.Append(
                    $"var {ToLower(result.FieldName)} = (await dataReader.Read{next}ResultAsync(Read{result.Name}, request.CancellationToken)).ToReadOnlyList();\r\n");
            }

            stringBuilder.Append($"result = new {_request.Name}DbQueryResult({GetResultVariableNames()});");
            return stringBuilder.ToString();
        }

        private string GetQueryResultClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"public sealed class {_request.Name}DbQueryResult\r\n{{\r\n");

            var sequence = new Sequence();
            foreach (var result in _request.Results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n");

                stringBuilder.Append($"    public readonly ReadOnlyList<{result.Name}> {result.FieldName};");
            }

            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetQueryResultClassConstructor(_request.Results).Indent(1));
            stringBuilder.Append("\r\n}");
            return stringBuilder.ToString();
        }

        private string GetQueryResultClassConstructor(ReadOnlyList<DbQueryResult> results)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"public {_request.Name}DbQueryResult(");

            var sequence = new Sequence();
            foreach (var result in results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append($"ReadOnlyList<{result.Name}> {ToLower(result.FieldName)}");
            }

            stringBuilder.Append(")\r\n");
            stringBuilder.Append("{\r\n");

            foreach (var result in results)
                stringBuilder.Append($"    {result.FieldName} = {ToLower(result.FieldName)};\r\n");

            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        private string GetResultVariableNames() => string.Join(", ", _request.Results.Select(i => ToLower(i.FieldName)));

        private string GetRecordClasses()
        {
            var stringBuilder = new StringBuilder();
            var sequence = new Sequence();
            foreach (var result in _request.Results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n\r\n");

                var recordClass = GetRecordClass(result);
                stringBuilder.Append(recordClass);
            }

            return stringBuilder.ToString();
        }

        private string GetRequestType() => _request.Results.Count == 0 ? "Command" : "Query";

        private string GetRequestClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"public sealed class {_request.Name}Db{GetRequestType()}
{{
");
            foreach (var parameter in _request.Parameters)
                stringBuilder.Append(
                    $"    public readonly {GetCSharpTypeName(parameter.SqlDbType, parameter.DataType, parameter.IsNullable)} {ToUpper(parameter.Name)};\r\n");

            stringBuilder.Append("\r\n");
            stringBuilder.Append(GetRequestClassConstructor().Indent(1));
            stringBuilder.Append("\r\n}");
            return stringBuilder.ToString();
        }

        private string GetRequestClassConstructor()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"public {_request.Name}Db{GetRequestType()}(");

            var sequence = new Sequence();
            foreach (var parameter in _request.Parameters)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append(
                    $"{GetCSharpTypeName(parameter.SqlDbType, parameter.DataType, parameter.IsNullable)} {parameter.Name}");
            }

            stringBuilder.Append(")\r\n{\r\n");

            foreach (var parameter in _request.Parameters)
                stringBuilder.Append($"    {ToUpper(parameter.Name)} = {parameter.Name};\r\n");

            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        private static string GetReadRecordMethod(DbQueryResult result)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"private static {result.Name} Read{result.Name}(IDataRecord dataRecord)\r\n");
            stringBuilder.Append("{\r\n");

            var sequence = new Sequence();
            foreach (var field in result.Fields)
            {
                var index = sequence.Next();
                stringBuilder.Append($"    var {ToLower(field.Name)} = dataRecord.{GetDataRecordMethodName(field)}({index});\r\n");
            }

            stringBuilder.Append("\r\n");
            stringBuilder.Append($"    return new {result.Name}(");

            sequence.Reset();
            foreach (var field in result.Fields)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append($"{ToLower(field.Name)}");
            }

            stringBuilder.Append(");\r\n");
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        private static string GetRecordClass(DbQueryResult result)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("public sealed class ");
            stringBuilder.Append(result.Name);
            stringBuilder.Append("\r\n{\r\n");

            var sequence = new Sequence();
            foreach (var field in result.Fields)
            {
                if (sequence.Next() > 0)
                    stringBuilder.AppendLine();

                stringBuilder.Append("    public readonly ");
                stringBuilder.Append(GetCSharpTypeName(field.DataType, field.IsNullable));
                stringBuilder.Append(' ');
                stringBuilder.Append(field.Name);
                stringBuilder.Append(';');
            }

            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetRecordClassConstructor(result).Indent(1));
            stringBuilder.Append(@"
}");

            return stringBuilder.ToString();
        }

        private static string GetRecordClassConstructor(DbQueryResult result)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"public {result.Name}(");

            var sequence = new Sequence();
            foreach (var field in result.Fields)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append($"{GetCSharpTypeName(field.DataType, field.IsNullable)} {ToLower(field.Name)}");
            }

            stringBuilder.Append(")\r\n");
            stringBuilder.Append("{\r\n");

            foreach (var field in result.Fields)
                stringBuilder.Append($"    {field.Name} = {ToLower(field.Name)};\r\n");

            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        private string GetToCreateCommandRequestMethod()
        {
            return $@"private CreateCommandRequest ToCreateCommandRequest({_request.Name}Db{GetRequestType()} {ToLower(GetRequestType())})
{{
    var parameters = ToParameters({ToLower(GetRequestType())});
    return new CreateCommandRequest(CommandText, parameters, CommandType.Text, CommandTimeout, _transaction);
}}";
        }

        private string GetToExecuteReaderRequestMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"private ExecuteReaderRequest ToExecuteReaderRequest({_request.Name}DbQuery query, CancellationToken cancellationToken)
{{    
    var createCommandRequest = ToCreateCommandRequest(query);
    return new ExecuteReaderRequest(createCommandRequest, CommandBehavior.Default, cancellationToken);
}}");
            return stringBuilder.ToString();
        }

        private string GetToParametersMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(
                $"private static ReadOnlyList<object> ToParameters({_request.Name}Db{GetRequestType()} {ToLower(GetRequestType())})\r\n");
            stringBuilder.Append("{\r\n");
            stringBuilder.Append("    var parameters = new SqlParameterCollectionBuilder();\r\n");

            foreach (var parameter in _request.Parameters)
            {
                if (parameter.SqlDbType == SqlDbType.Structured)
                    stringBuilder.Append(
                        $"    parameters.AddStructured(\"{parameter.Name}\", \"{parameter.DataType}\", {ToLower(GetRequestType())}.{ToUpper(parameter.Name)}.Select(i => i.ToSqlDataRecord()).ToReadOnlyList());\r\n");
                else
                {
                    var method = parameter.SqlDbType == SqlDbType.Date ? "AddDate" : "Add";
                    stringBuilder.Append($"    parameters.{method}(\"{parameter.Name}\", {ToLower(GetRequestType())}.{ToUpper(parameter.Name)});\r\n");
                }
            }

            stringBuilder.Append("    return parameters.ToReadOnlyList();\r\n");
            stringBuilder.Append("}");

            return stringBuilder.ToString();
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

        private static string ToLower(string pascalCase)
        {
            return !pascalCase.IsNullOrEmpty()
                ? char.ToLower(pascalCase[0]) + pascalCase.Substring(1)
                : pascalCase;
        }

        private static string ToUpper(string camelCase)
        {
            return !camelCase.IsNullOrEmpty()
                ? char.ToUpper(camelCase[0]) + camelCase.Substring(1)
                : camelCase;
        }
    }
}