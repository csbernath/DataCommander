using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Foundation.Collections;
using Foundation.Core;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DbRequestBuilder
{
    private readonly DbRequest _request;

    public DbRequestBuilder(DbRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        _request = request;
    }

    public string Build()
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($@"using System;
using System.Collections.ObjectModel;
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
            string userDefinedTableType = dataType.Split('.')[1];
            csharpTypeName = $"ReadOnlyCollection<{userDefinedTableType}>";
        }
        else
        {
            csharpTypeName = SqlDataTypeRepository.SqlDataTypes.First(i => i.SqlDbType == sqlDbType).CSharpTypeName;
            if (csharpTypeName != CSharpTypeName.String && isNullable)
                csharpTypeName += "?";
        }

        return csharpTypeName;
    }

    private static string GetCSharpTypeName(Type dbColumnDataType, bool isNullable)
    {
        CSharpType csharpType = CSharpTypeArray.CSharpTypes.First(i => i.Type == dbColumnDataType);
        string csharpTypeName = csharpType.Name;

        if (isNullable && dbColumnDataType.IsValueType)
            csharpTypeName += '?';

        return csharpTypeName;
    }

    private static string GetDataRecordMethodName(DbQueryResultField field)
    {
        TypeCode typeCode = Type.GetTypeCode(field.DataType);
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
        string commandTimeoutString = _request.CommandTimeout != null ? _request.CommandTimeout.Value.ToString() : "null";

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"\r\n\r\npublic sealed class {_request.Name}Db{GetRequestType()}Handler\r\n{{\r\n");
        stringBuilder.Append("    private const string CommandText = @\"\";\r\n");
        stringBuilder.Append($"    private readonly int? CommandTimeout = {commandTimeoutString};\r\n");
        stringBuilder.Append("    private readonly IDbConnection _connection;\r\n");
        stringBuilder.Append("    private readonly IDbTransaction _transaction;\r\n\r\n");
        stringBuilder.Append($"    public {_request.Name}Db{GetRequestType()}Handler(IDbConnection connection, IDbTransaction transaction)\r\n");
        stringBuilder.Append("    {\r\n");
        stringBuilder.Append("        ArgumentNullException.ThrowIfNull(connection);\r\n");
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

            stringBuilder.Append("\r\n\r\n");

            Sequence sequence = new Sequence();
            foreach (DbQueryResult result in _request.Results)
            {
                if (sequence.Next() > 0)
                    stringBuilder.Append("\r\n\r\n");

                string readMethod = GetReadRecordMethod(result);
                stringBuilder.Append(readMethod.Indent(1));
            }
        }

        stringBuilder.Append("\r\n}");
        return stringBuilder.ToString();
    }

    private string GetExecuteReaderAsyncMethod()
    {
        StringBuilder stringBuilder = new StringBuilder();
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
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($@"private {_request.Name}DbQueryResult ExecuteReader(ExecuteReaderRequest request)
{{
    {_request.Name}DbQueryResult result = null;
    var executor = _connection.CreateCommandExecutor();
    executor.ExecuteReader(request, dataReader =>
    {{
");
        Sequence sequence = new Sequence();
        foreach (DbQueryResult result in _request.Results)
        {
            int index = sequence.Next();
            string next = null;
            if (index > 0)
            {
                stringBuilder.Append("\r\n");
                next = "Next";
            }

            stringBuilder.Append(
                $"        var {result.FieldName.ToCamelCase()} = dataReader.Read{next}Result(128, Read{result.Name});");
        }

        stringBuilder.Append($"\r\n        result = new {_request.Name}DbQueryResult(");

        sequence.Reset();
        foreach (DbQueryResult result in _request.Results)
        {
            if (sequence.Next() > 0)
                stringBuilder.Append(", ");

            stringBuilder.Append($"{result.FieldName.ToCamelCase()}");
        }

        stringBuilder.Append(");\r\n");
        stringBuilder.Append("    });\r\n\r\n");
        stringBuilder.Append("    return result;\r\n");
        stringBuilder.Append('}');
        return stringBuilder.ToString();
    }

    private string GetHandleMethod()
    {
        string responseType = _request.Results.Count == 0 ? "int" : $"{_request.Name}DbQueryResult";
        string requestParameter = _request.Results.Count == 0 ? "command" : "query";

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($@"public {responseType} Handle({_request.Name}Db{GetRequestType()} {requestParameter})
{{
    ArgumentNullException.ThrowIfNull({requestParameter});
");

        if (_request.Results.Count == 0)
        {
            stringBuilder.Append($@"    var request = ToCreateCommandRequest({GetRequestType().ToCamelCase()});
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
        string request = _request.Results.Count == 0 ? "command" : "query";
        string result = _request.Results.Count == 0 ? "int" : $"{_request.Name}DbQueryResult";
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($@"public Task<{result}> HandleAsync({_request.Name}Db{GetRequestType()} {request}, CancellationToken cancellationToken)
{{
    ArgumentNullException.ThrowIfNull({request});
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
        StringBuilder stringBuilder = new StringBuilder();
        Sequence sequence = new Sequence();
        foreach (DbQueryResult result in _request.Results)
        {
            string next = sequence.Next() == 0 ? null : "Next";
            stringBuilder.Append(
                $"var {result.FieldName.ToCamelCase()} = (await dataReader.Read{next}ResultAsync(128, Read{result.Name}, request.CancellationToken));\r\n");
        }

        stringBuilder.Append($"result = new {_request.Name}DbQueryResult({GetResultVariableNames()});");
        return stringBuilder.ToString();
    }

    private string GetQueryResultClass()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"public sealed class {_request.Name}DbQueryResult\r\n{{\r\n");

        Sequence sequence = new Sequence();
        foreach (DbQueryResult result in _request.Results)
        {
            if (sequence.Next() > 0)
                stringBuilder.Append("\r\n");

            stringBuilder.Append($"    public readonly ReadOnlySegmentLinkedList<{result.Name}> {result.FieldName};");
        }

        stringBuilder.Append("\r\n\r\n");
        stringBuilder.Append(GetQueryResultClassConstructor(_request.Results).Indent(1));
        stringBuilder.Append("\r\n}");
        return stringBuilder.ToString();
    }

    private string GetQueryResultClassConstructor(ReadOnlyCollection<DbQueryResult> results)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($"public {_request.Name}DbQueryResult(");

        Sequence sequence = new Sequence();
        foreach (DbQueryResult result in results)
        {
            if (sequence.Next() > 0)
                stringBuilder.Append(", ");

            stringBuilder.Append($"ReadOnlySegmentLinkedList<{result.Name}> {result.FieldName.ToCamelCase()}");
        }

        stringBuilder.Append(")\r\n");
        stringBuilder.Append("{\r\n");

        foreach (DbQueryResult result in results)
            stringBuilder.Append($"    {result.FieldName} = {result.FieldName.ToCamelCase()};\r\n");

        stringBuilder.Append('}');

        return stringBuilder.ToString();
    }

    private string GetResultVariableNames() => string.Join(", ", _request.Results.Select(i => i.FieldName.ToCamelCase()));

    private string GetRecordClasses()
    {
        StringBuilder stringBuilder = new StringBuilder();
        Sequence sequence = new Sequence();
        foreach (DbQueryResult result in _request.Results)
        {
            if (sequence.Next() > 0)
                stringBuilder.Append("\r\n\r\n");

            string recordClass = GetRecordClass(result);
            stringBuilder.Append(recordClass);
        }

        return stringBuilder.ToString();
    }

    private string GetRequestType() => _request.Results.Count == 0 ? "Command" : "Query";

    private string GetRequestClass()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($@"public sealed class {_request.Name}Db{GetRequestType()}
{{
");
        if (_request.Parameters.Count > 0)
        {
            foreach (DbRequestParameter parameter in _request.Parameters)
                stringBuilder.Append(
                    $"    public readonly {GetCSharpTypeName(parameter.SqlDbType, parameter.DataType, parameter.IsNullable)} {parameter.Name.ToPascalCase()};\r\n");

            stringBuilder.Append("\r\n");
        }

        stringBuilder.Append(GetRequestClassConstructor().Indent(1));
        stringBuilder.Append("\r\n}");
        return stringBuilder.ToString();
    }

    private string GetRequestClassConstructor()
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"public {_request.Name}Db{GetRequestType()}(");

        Sequence sequence = new Sequence();
        foreach (DbRequestParameter parameter in _request.Parameters)
        {
            if (sequence.Next() > 0)
                stringBuilder.Append(", ");

            stringBuilder.Append(
                $"{GetCSharpTypeName(parameter.SqlDbType, parameter.DataType, parameter.IsNullable)} {parameter.Name}");
        }

        stringBuilder.Append(")\r\n{\r\n");

        foreach (DbRequestParameter parameter in _request.Parameters)
            stringBuilder.Append($"    {parameter.Name.ToPascalCase()} = {parameter.Name};\r\n");

        stringBuilder.Append('}');

        return stringBuilder.ToString();
    }

    private static string GetReadRecordMethod(DbQueryResult result)
    {
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"private static {result.Name} Read{result.Name}(IDataRecord dataRecord)\r\n");
        stringBuilder.Append("{\r\n");

        Sequence sequence = new Sequence();
        foreach (DbQueryResultField field in result.Fields)
        {
            int index = sequence.Next();
            stringBuilder.Append($"    var {field.Name.ToCamelCase()} = dataRecord.{GetDataRecordMethodName(field)}({index});\r\n");
        }

        stringBuilder.Append("\r\n");
        stringBuilder.Append($"    return new {result.Name}(");

        sequence.Reset();
        foreach (DbQueryResultField field in result.Fields)
        {
            if (sequence.Next() > 0)
                stringBuilder.Append(", ");

            stringBuilder.Append($"{field.Name.ToCamelCase()}");
        }

        stringBuilder.Append(");\r\n");
        stringBuilder.Append('}');

        return stringBuilder.ToString();
    }

    private static string GetRecordClass(DbQueryResult result)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("public sealed class ");
        stringBuilder.Append(result.Name);
        stringBuilder.Append("\r\n{\r\n");

        Sequence sequence = new Sequence();
        foreach (DbQueryResultField field in result.Fields)
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
        StringBuilder stringBuilder = new StringBuilder();

        stringBuilder.Append($"public {result.Name}(");

        Sequence sequence = new Sequence();
        foreach (DbQueryResultField field in result.Fields)
        {
            if (sequence.Next() > 0)
                stringBuilder.Append(", ");

            stringBuilder.Append($"{GetCSharpTypeName(field.DataType, field.IsNullable)} {field.Name.ToCamelCase()}");
        }

        stringBuilder.Append(")\r\n");
        stringBuilder.Append("{\r\n");

        foreach (DbQueryResultField field in result.Fields)
            stringBuilder.Append($"    {field.Name} = {field.Name.ToCamelCase()};\r\n");

        stringBuilder.Append('}');

        return stringBuilder.ToString();
    }

    private string GetToCreateCommandRequestMethod()
    {
        return $@"private CreateCommandRequest ToCreateCommandRequest({_request.Name}Db{GetRequestType()} {GetRequestType().ToCamelCase()})
{{
    var parameters = ToParameters({GetRequestType().ToCamelCase()});
    return new CreateCommandRequest(CommandText, parameters, CommandType.Text, CommandTimeout, _transaction);
}}";
    }

    private string GetToExecuteReaderRequestMethod()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append($@"private ExecuteReaderRequest ToExecuteReaderRequest({_request.Name}DbQuery query, CancellationToken cancellationToken)
{{    
    var createCommandRequest = ToCreateCommandRequest(query);
    return new ExecuteReaderRequest(createCommandRequest, CommandBehavior.Default, cancellationToken);
}}");
        return stringBuilder.ToString();
    }

    private string GetToParametersMethod()
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(
            $"private static ReadOnlyCollection<object> ToParameters({_request.Name}Db{GetRequestType()} {GetRequestType().ToCamelCase()})\r\n");
        stringBuilder.Append("{\r\n");
        stringBuilder.Append("    var parameters = new SqlParameterCollectionBuilder();\r\n");

        foreach (DbRequestParameter parameter in _request.Parameters)
        {
            if (parameter.SqlDbType == SqlDbType.Structured)
                stringBuilder.Append(
                    $"    parameters.AddStructured(\"{parameter.Name}\", \"{parameter.DataType}\", {GetRequestType().ToCamelCase()}.{parameter.Name.ToPascalCase()}.Select(i => i.ToSqlDataRecord()).ToReadOnlyCollection());\r\n");
            else if (parameter.SqlDbType == SqlDbType.Char)
                stringBuilder.Append(
                    $"    parameters.AddChar(\"{parameter.Name}\", {parameter.Size}, {GetRequestType().ToCamelCase()}.{parameter.Name.ToPascalCase()});\r\n");
            else if (parameter.SqlDbType == SqlDbType.NVarChar)
                stringBuilder.Append(
                    $"    parameters.AddNVarChar(\"{parameter.Name}\", {parameter.Size}, {GetRequestType().ToCamelCase()}.{parameter.Name.ToPascalCase()});\r\n");
            else if (parameter.SqlDbType == SqlDbType.VarChar)
                stringBuilder.Append(
                    $"    parameters.AddVarChar(\"{parameter.Name}\", {parameter.Size}, {GetRequestType().ToCamelCase()}.{parameter.Name.ToPascalCase()});\r\n");
            else
            {
                string method = parameter.SqlDbType switch
                {
                    SqlDbType.Bit => !parameter.IsNullable ? "Add" : "AddNullableBit",
                    SqlDbType.Date => !parameter.IsNullable ? "AddDate" : "AddNullableDate",
                    SqlDbType.DateTime => !parameter.IsNullable ? "Add" : "AddNullableDateTime",
                    SqlDbType.Int => !parameter.IsNullable ? "Add" : "AddNullableInt",
                    SqlDbType.UniqueIdentifier => !parameter.IsNullable ? "Add" : "AddNullableGuid",
                    SqlDbType.VarChar => "AddString",
                    SqlDbType.Xml => "AddXml",
                    _ => "Add",
                };
                stringBuilder.Append($"    parameters.{method}(\"{parameter.Name}\", {GetRequestType().ToCamelCase()}.{parameter.Name.ToPascalCase()});\r\n");
            }
        }

        stringBuilder.Append("    return parameters.ToReadOnlyCollection();\r\n");
        stringBuilder.Append('}');

        return stringBuilder.ToString();
    }
}