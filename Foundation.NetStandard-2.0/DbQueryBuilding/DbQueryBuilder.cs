using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Foundation.Assertions;
using Foundation.Data.SqlClient;

namespace Foundation.DbQueryBuilding
{
    public sealed class DbQueryBuilder
    {
        private readonly DbQuery _query;

        public DbQueryBuilder(DbQuery query)
        {
            Assert.IsNotNull(query);
            _query = query;
        }

        public string Build()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($@"using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
{_query.Using}

namespace {_query.Namespace}
{{
");

            stringBuilder.Append(GetQueryClass().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetQueryResultClass().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetRecordClasses().Indent(1));
            stringBuilder.Append(GetHandlerClass().Indent(1).Replace("\"\"", $"\"{_query.CommandText}\""));
            stringBuilder.Append("\r\n}");

            return stringBuilder.ToString();
        }

        private string GetHandlerClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"\r\n\r\npublic sealed class {_query.Name}DbQueryHandler\r\n{{\r\n");
            stringBuilder.Append("    private const string CommandText = @\"\";\r\n");
            stringBuilder.Append("    private readonly IDbConnection _connection;\r\n");
            stringBuilder.Append("    private readonly IDbTransaction _transaction;\r\n\r\n");
            stringBuilder.Append($"    public {_query.Name}DbQueryHandler(IDbConnection connection, IDbTransaction transaction)\r\n");
            stringBuilder.Append("    {\r\n");
            stringBuilder.Append("        Assert.IsNotNull(connection);\r\n");
            stringBuilder.Append("        _connection = connection;\r\n");
            stringBuilder.Append("        _transaction = transaction;\r\n");
            stringBuilder.Append("    }\r\n\r\n");
            stringBuilder.Append(GetHandleQueryMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetHandleQueryAsyncMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetToExecuteReaderRequestMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetToParametersMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetExecuteReaderMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetExecuteReaderAsyncMethod().Indent(1));
            stringBuilder.Append("\r\n\r\n");

            var sequence = new Sequence();
            foreach (var result in _query.Results)
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
        private static string ToLower(string pascalCase) => char.ToLower(pascalCase[0]) + pascalCase.Substring(1);

        private string GetHandleQueryMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"public {_query.Name}DbQueryResult Handle({_query.Name}DbQuery query)
{{
    Assert.IsNotNull(query);
    var request = ToExecuteReaderRequest(query, CancellationToken.None);
    return ExecuteReader(request);
}}");
            return stringBuilder.ToString();
        }

        private string GetHandleQueryAsyncMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"public Task<{_query.Name}DbQueryResult> HandleAsync({_query.Name}DbQuery query, CancellationToken cancellationToken)
{{
    Assert.IsNotNull(query);
    var request = ToExecuteReaderRequest(query, cancellationToken);
    return ExecuteReaderAsync(request);
}}");
            return stringBuilder.ToString();
        }

        private string GetToExecuteReaderRequestMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"private ExecuteReaderRequest ToExecuteReaderRequest({_query.Name}DbQuery query, CancellationToken cancellationToken)
{{
    var parameters = ToParameters(query);
    const int commandTimeout = 0;
    var createCommandRequest = new CreateCommandRequest(CommandText, parameters, CommandType.Text, commandTimeout, _transaction);
    return new ExecuteReaderRequest(createCommandRequest, CommandBehavior.Default, cancellationToken);
}}");
            return stringBuilder.ToString();
        }

        private string GetToParametersMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"private static ReadOnlyCollection<object> ToParameters({_query.Name}DbQuery query)\r\n");
            stringBuilder.Append("{\r\n");
            stringBuilder.Append("    var parameters = new SqlParameterCollectionBuilder();\r\n");

            foreach (var parameter in _query.Parameters)
            {
                if (parameter.SqlDbType == SqlDbType.Structured)
                    stringBuilder.Append($"    parameters.AddStructured(\"{parameter.Name}\", \"{parameter.DataType}\", {parameter.CSharpValue});\r\n");
                else
                {
                    var method = parameter.SqlDbType == SqlDbType.Date ? "AddDate" : "Add";
                    stringBuilder.Append($"    parameters.{method}(\"{parameter.Name}\", query.{ToUpper(parameter.Name)});\r\n");
                }
            }

            stringBuilder.Append("    return parameters.ToReadOnlyCollection();\r\n");
            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        private string GetExecuteReaderMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"private {_query.Name}DbQueryResult ExecuteReader(ExecuteReaderRequest request)
{{
    {_query.Name}DbQueryResult result = null;
    var executor = _connection.CreateCommandExecutor();
    executor.ExecuteReader(request, dataReader =>
    {{
");
            var sequence = new Sequence();
            foreach (var result in _query.Results)
            {
                var index = sequence.Next();
                string next = null;
                if (index > 0)
                {
                    stringBuilder.Append("\r\n");
                    next = "Next";
                }

                stringBuilder.Append($"        var {ToLower(result.FieldName)} = dataReader.Read{next}Result(Read{result.Name}).AsReadOnly();");
            }

            stringBuilder.Append($"\r\n        result = new {_query.Name}DbQueryResult(");

            sequence.Reset();
            foreach (var result in _query.Results)
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

        private string GetExecuteReaderAsyncMethod()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"private async Task<{_query.Name}DbQueryResult> ExecuteReaderAsync(ExecuteReaderRequest request)
{{
    {_query.Name}DbQueryResult result = null;
    var connection = (DbConnection) _connection;
    var executor = connection.CreateCommandAsyncExecutor();
    await executor.ExecuteReaderAsync(request, async dataReader =>
    {{
{GetExecuteReaderAsyncMethodFragment().Indent(2)}
    }});

    return result;
}}");
            return stringBuilder.ToString();
        }

        private string GetExecuteReaderAsyncMethodFragment()
        {
            var stringBuilder = new StringBuilder();
            var sequence = new Sequence();
            foreach (var result in _query.Results)
            {
                var next = sequence.Next() == 0 ? null : "Next";
                stringBuilder.Append(
                    $"var {ToLower(result.FieldName)} = (await dataReader.Read{next}ResultAsync(Read{result.Name}, request.CancellationToken)).AsReadOnly();\r\n");
            }

            stringBuilder.Append($"result = new {_query.Name}DbQueryResult({GetResultVariableNames()});");
            return stringBuilder.ToString();
        }

        private string GetResultVariableNames() => string.Join(", ", _query.Results.Select(i => ToLower(i.FieldName)));

        private string GetRecordClasses()
        {
            var stringBuilder = new StringBuilder();
            var sequence = new Sequence();
            foreach (var result in _query.Results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n\r\n");

                var recordClass = GetRecordClass(result);
                stringBuilder.Append(recordClass);
            }

            return stringBuilder.ToString();
        }

        private string GetQueryClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($@"public sealed class {_query.Name}DbQuery
{{
");
            foreach (var parameter in _query.Parameters)
                stringBuilder.Append(
                    $"    public readonly {GetCSharpTypeName(parameter.SqlDbType, parameter.CSharpDataType, parameter.IsNullable)} {ToUpper(parameter.Name)};\r\n");

            stringBuilder.Append("\r\n");
            stringBuilder.Append(GetQueryClassConstructor().Indent(1));
            stringBuilder.Append("\r\n}");
            return stringBuilder.ToString();
        }

        private string GetQueryClassConstructor()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append($"public {_query.Name}DbQuery(");

            var sequence = new Sequence();
            foreach (var parameter in _query.Parameters)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append(
                    $"{GetCSharpTypeName(parameter.SqlDbType, parameter.CSharpDataType, parameter.IsNullable)} {parameter.Name}");
            }

            stringBuilder.Append(")\r\n{\r\n");

            foreach (var parameter in _query.Parameters)
                stringBuilder.Append($"    {ToUpper(parameter.Name)} = {parameter.Name};\r\n");

            stringBuilder.Append("}");

            return stringBuilder.ToString();
        }

        private string GetQueryResultClass()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"public sealed class {_query.Name}DbQueryResult\r\n{{\r\n");

            var sequence = new Sequence();
            foreach (var result in _query.Results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n");

                stringBuilder.Append($"    public readonly ReadOnlyCollection<{result.Name}> {result.FieldName};");
            }

            stringBuilder.Append("\r\n\r\n");
            stringBuilder.Append(GetQueryResultClassConstructor(_query.Results).Indent(1));
            stringBuilder.Append("\r\n}");
            return stringBuilder.ToString();
        }

        private string GetQueryResultClassConstructor(ReadOnlyCollection<DbQueryResult> results)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"public {_query.Name}DbQueryResult(");

            var sequence = new Sequence();
            foreach (var result in results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append(", ");

                stringBuilder.Append($"ReadOnlyCollection<{result.Name}> {ToLower(result.FieldName)}");
            }

            stringBuilder.Append(")\r\n");
            stringBuilder.Append("{\r\n");

            foreach (var result in results)
                stringBuilder.Append($"    {result.FieldName} = {ToLower(result.FieldName)};\r\n");

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

        private static string GetCSharpTypeName(SqlDbType sqlDbType, string csharpDataType, bool isNullable)
        {
            string csharpTypeName;
            if (sqlDbType == SqlDbType.Structured)
                csharpTypeName = csharpDataType;
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
    }
}